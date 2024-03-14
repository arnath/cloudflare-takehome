using Cfth.Data;
using Cfth.Handlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Sydney.Core;
using System.Net;

namespace Cfth.Test;

public class RedirectRestHandlerTests
{
    private readonly ILoggerFactory loggerFactory;
    private readonly IUrlRepository urlRepository;
    private readonly ICodec codec;

    public RedirectRestHandlerTests()
    {
        this.loggerFactory = new LoggerFactory();
        this.urlRepository = new InMemoryUrlRepository();
        this.codec = new Base62Codec();
    }
    
    [Fact]
    public async void GetThrows400WhenIdIsInvalid()
    {
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["encodedId"]).Returns("asdf*#32");

        RedirectRestHandler handler =
            new RedirectRestHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec);

        HttpResponseException exception =
            await Assert.ThrowsAsync<HttpResponseException>(
                () => handler.GetAsync(request));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async void GetThrows404WhenEntityDoesNotExist()
    {
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["encodedId"]).Returns("n");

        RedirectRestHandler handler =
            new RedirectRestHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec);

        HttpResponseException exception =
            await Assert.ThrowsAsync<HttpResponseException>(
                () => handler.GetAsync(request));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async void GetThrows404WhenEntityIsExpired()
    {
        UrlEntity entity =
            new UrlEntity(
                new Uri("https://www.google.com"),
                DateTime.Now.AddMonths(-1));
        await this.urlRepository.PutAsync(entity);
        
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["encodedId"]).Returns("n");

        RedirectRestHandler handler =
            new RedirectRestHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec);

        HttpResponseException exception =
            await Assert.ThrowsAsync<HttpResponseException>(
                () => handler.GetAsync(request));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async void GetReturns302WhenSuccessful()
    {
        UrlEntity entity =
            new UrlEntity(
                new Uri("https://www.google.com"),
                DateTime.Now.AddMonths(1));
        await this.urlRepository.PutAsync(entity);

        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["encodedId"]).Returns(this.codec.Encode(entity.Id));

        RedirectRestHandler handler =
            new RedirectRestHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec);
        SydneyResponse response = await handler.GetAsync(request);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal(entity.Url!.ToString(), response.Headers["Location"]);
    }

    [Fact]
    public async void GetIncrementsUsageCounterWhenSuccessful()
    {
        UrlEntity? entity =
            new UrlEntity(
                new Uri("https://www.google.com"));
        await this.urlRepository.PutAsync(entity);

        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["encodedId"]).Returns(this.codec.Encode(entity.Id));

        RedirectRestHandler handler =
            new RedirectRestHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec);

        Assert.Equal(0UL, entity.UsageCounter!.AllTime);
        Assert.Equal(0UL, entity.UsageCounter!.LastWeek);
        Assert.Equal(0UL, entity.UsageCounter!.LastDay);

        await handler.GetAsync(request);
        await handler.GetAsync(request);

        entity = await this.urlRepository.GetAsync(entity.Id);
        Assert.NotNull(entity);

        Assert.Equal(2UL, entity.UsageCounter!.AllTime);
        Assert.Equal(2UL, entity.UsageCounter!.LastWeek);
        Assert.Equal(2UL, entity.UsageCounter!.LastDay);
    }
}

