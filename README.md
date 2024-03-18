# Cloudflare Takehome Assignment: URL Shortener

This project is a URL shortener service, written in C# using .NET 8. I built it as part of
Cloudflare's interview process for a position in March 2024. 

## Project Overview

This project exposes a console app that runs a REST service for the URL shortener. The service
is in the `src` directory and the unit tests are in the `test` directory. When run, the
service listens for incoming HTTP requests on `http://localhost:8080`. It exposes
the following API endpoints:

### Create URL

Creates a new shortened URL.

```
POST /urls/

// Request body
{
  // The URL to be shortened
  "url": "https://www.google.com",
  // Optional expiration in ISO 8601 format
  "expiresAt": "2024-12-31"
}

// Response body
{
  "id": 1,
  "originalUrl": "https://www.google.com",
  "shortenedUrl": "http://localhost:8080/n",
  // Optional expiration, can be null
  "expiresAt": "2024-12-31",
  "usage":
  {
    // Each of these us the number of uses in the last time span.
    "lastDay": 1,
    "lastWeek": 2,
    "allTime": 10
  }
}
```

### Get URL

Gets the details of a shortened URL by id.

```
GET /urls/{id}

// Response body
{
  "id": 1,
  "originalUrl": "https://www.google.com",
  "shortenedUrl": "http://localhost:8080/n",
  // Optional expiration, can be null
  "expiresAt": "2024-12-31",
  "usage":
  {
    // Each of these us the number of uses in the last time span.
    "lastDay": 1,
    "lastWeek": 2,
    "allTime": 10
  }
}
```

### Delete URL

Deletes a URL by id.

```
DELETE /urls/{id}

// Returns HTTP 202 Accepted regardless of whether the item was deleted.
```

### Redirect URL

Given a short URL, redirects to the original URL. This API has the following additional behaviors:

- It increments the usage statistics for this URL.
- If the URL is expired, it returns 404 instead of redirecting.

```
GET /{encodedId}

// Returns HTTP 302 Found if this was a mapped URL. If not, returns HTTP 404 Not Found.
```

## Design Decisions

### Language and framework

C# is the language I'm most comfortable in and one I love using so that was an easy choice. The
default REST framework choice for .NET is ASP.NET Core. However, I don't really like ASP.NET for
a number of reasons, primarily because there's too much magic involved. I find it hard to debug
flows through my application.

As such, I went with a REST framework I wrote myself called [Sydney][sydney]. There are more
details in its repo but it's designed for small projects like this and to be easy to use. It was
also a nice option because I got to see it in action and make some improvements around testability
of services writteh with Sydney. 

### How to shorten URLs

I did some digging on how this is normally done and was pointed toward base62 encoding (which is
the entire alphanumeric character set). When a new URL gets written to storage in the service, 
it gets assigned an ID. We then base62 encode this ID to get the shortened URL slug. 

### Storage

As of right now, the service offers two storage layers (implementers of `IUrlRepository`): an
in-memory storage repo that keeps URLs in a dictionary and a SQLite + EF Core based storage repo.
Abstracting away the storage layer with this interface was necessary to make things testable and
make it possible for me to use and provide multiple storage options. I went with SQLite (and 
SQL over NoSQL) primarily for simplicity. The set of data we're storing here is small and doesn't
have any relations so there's no particular advantage to one over the other. 

### How to track usage

The instructions were not super-specific on whether usage tracking should be an application feature
(i.e., something stored in the database) or part of monitoring (i.e., implemented as metrics and
viewed through some external tool). I decided to go with making it an application feature because
given the way the metrics thing was described, it seemed more like an optional feature. It also
would have involved some additional tools. Therefore, the usage tracker is stored in the database
alongside the URLs. 

For implementation, I went with keeping 3 "buckets": one counter for all time usage, a list of
usages within the last week, and a list of usages within the last day. I felt like this offered a
good tradeoff of simplicity vs performance. For a single shortened URL, the usage for a day or
week is likely to be relatively small. Doing the list pruning and the like that's in the
`UsageCounter` class won't have a huge performance hit. A rolling window with 1 hour buckets and
1 day buckets (for the last day and last week respectively) would be higher performance but
harder to implement.

## Building and Deployment

To build and deploy this project, you must install the [.NET SDK and CLI][dotnet]. After that,
clone the repo and follow the steps below.

### Building and testing

Run the following commands from a terminal in the folder where you cloned the repo:

```
dotnet restore
dotnet build --no-restore

# Runs the tests
dotnet test --no-build

# Runs the service locally
dotnet run --project src/Cfth.csproj
```

### Create a deployment bundle

Run the following commands from a terminal in the folder where you cleaned the repo:

```
dotnet restore
dotnet build --no-restore --configuration Release 

# Publish a deployable bundle to the output folder
dotnet publish src/Cfth.csproj --no-restore --no-build --configuration Release --output ./dist/
```

Then simply copy the `dist` folder to whatever deployment service you use.

[sydney]: https://github.com/arnath/sydney
[dotnet]: https://dotnet.microsoft.com/en-us/download
