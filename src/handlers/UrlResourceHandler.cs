using System.Net;
using Cfth.Data;
using Microsoft.Extensions.Logging;
using Sydney.Core;

namespace Cfth.Handlers;

public class UrlResourceHandler : ResourceHandlerBase
{
    private readonly IUrlRepository urlRepository;
    private readonly ICodec codec;

    public UrlResourceHandler(
        ILoggerFactory loggerFactory,
        IUrlRepository urlRepository,
        ICodec codec) : base(loggerFactory)
    {
        this.urlRepository = urlRepository;
        this.codec = codec;
    }

    public override async Task<SydneyResponse> CreateAsync(SydneyRequest request)
    {
        CreateUrlRequest requestBody = await request.DeserializeJsonAsync<CreateUrlRequest>();

        UrlEntity entity = new UrlEntity(requestBody.Url, requestBody.ExpiresAt);
        await this.urlRepository.PutAsync(entity);

        return new SydneyResponse(
            HttpStatusCode.OK,
            new UrlEntityResponse(
                this.codec,
                new Uri("http://www.localhost:8080"),
                entity));
    }

    public override async Task<SydneyResponse> GetAsync(SydneyRequest request)
    {
        string urlIdString = request.PathParameters["id"];
        if (!ulong.TryParse(urlIdString, out ulong id))
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid URL ID.");
        }

        UrlEntity? entity = await this.urlRepository.GetAsync(id);
        if (entity == null)
        {
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        return new SydneyResponse(
            HttpStatusCode.OK,
            new UrlEntityResponse(
                this.codec,
                new Uri("http://www.localhost:8080"),
                entity));
    }

    public override async Task<SydneyResponse> DeleteAsync(SydneyRequest request)
    {
        string urlIdString = request.PathParameters["id"];
        if (!ulong.TryParse(urlIdString, out ulong id))
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid URL ID.");
        }

        await this.urlRepository.DeleteAsync(id);

        return new SydneyResponse(HttpStatusCode.Accepted);
    }
}
