namespace Cfth.Test;

using System.Net;
using Cfth.Data;
using Cfth.Handlers;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Sydney.Core;
using Xunit;

public class UrlResourceHandlerTests
{
    private readonly ILoggerFactory loggerFactory;
    private readonly IUrlRepository urlRepository;
    private readonly ICodec codec;
    private readonly Uri originalUrl;
    private readonly Uri baseUrl;

    public UrlResourceHandlerTests()
    {
        this.loggerFactory = new LoggerFactory();
        this.urlRepository = new InMemoryUrlRepository();
        this.codec = new Base62Codec();
        this.originalUrl = new Uri("https://www.google.com");
        this.baseUrl = new Uri("http://localhost:8080");
    }

    [Fact]
    public async void CreateReturns200WhenSuccessful()
    {
        CreateUrlRequest createUrlRequest =
            new CreateUrlRequest(
                this.originalUrl,
                DateTime.Now.AddMonths(1));
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.DeserializeJsonAsync<CreateUrlRequest>()).Returns(createUrlRequest);

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);
        SydneyResponse response = await handler.CreateAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        UrlEntityResponse responseBody = Assert.IsType<UrlEntityResponse>(response.Payload);
        Assert.Equal(this.originalUrl, responseBody.OriginalUrl);
        Assert.Equal(new Uri(this.baseUrl, this.codec.Encode(responseBody.Id)), responseBody.ShortenedUrl);
        Assert.Equal(createUrlRequest.ExpiresAt, responseBody.ExpiresAt);
        Assert.NotNull(responseBody.Usage);
    }

    [Fact]
    public async void GetThrows400WhenIdIsInvalid()
    {
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["id"]).Returns("qwer");

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);

        HttpResponseException exception =
            await Assert.ThrowsAsync<HttpResponseException>(
                () => handler.GetAsync(request));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async void GetThrows404WhenEntityDoesNotExist()
    {
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["id"]).Returns("5");

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);

        HttpResponseException exception =
            await Assert.ThrowsAsync<HttpResponseException>(
                () => handler.GetAsync(request));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async void GetReturns200WhenSuccessful()
    {
        UrlEntity entity = new UrlEntity(this.originalUrl, DateTime.Now.AddMonths(1));
        await this.urlRepository.PutAsync(entity);

        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["id"]).Returns(entity.Id.ToString());

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);
        SydneyResponse response = await handler.GetAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        UrlEntityResponse responseBody = Assert.IsType<UrlEntityResponse>(response.Payload);

        Assert.Equal(entity.Id, responseBody.Id);
        Assert.Equal(entity.Url, responseBody.OriginalUrl);
        Assert.Equal(entity.ExpiresAt, responseBody.ExpiresAt);
    }

    [Fact]
    public async void DeleteThrows400WhenIdIsInvalid()
    {
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["id"]).Returns("qwer");

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);

        HttpResponseException exception =
            await Assert.ThrowsAsync<HttpResponseException>(
                () => handler.DeleteAsync(request));
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async void DeleteReturns202WhenEntityDoesNotExist()
    {
        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["id"]).Returns("5");

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);
        SydneyResponse response = await handler.DeleteAsync(request);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Null(response.Payload);
    }

    [Fact]
    public async void DeleteReturns202WhenSuccessful()
    {
        UrlEntity entity = new UrlEntity(this.originalUrl);
        await this.urlRepository.PutAsync(entity);

        ISydneyRequest request = A.Fake<ISydneyRequest>();
        A.CallTo(() => request.PathParameters["id"]).Returns(entity.Id.ToString());

        UrlResourceHandler handler =
            new UrlResourceHandler(
                this.loggerFactory,
                this.urlRepository,
                this.codec,
                this.baseUrl);
        SydneyResponse response = await handler.DeleteAsync(request);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        Assert.Null(response.Payload);
    }
}
