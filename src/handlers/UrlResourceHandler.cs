using System.Net;
using Cfth.Data;
using Microsoft.Extensions.Logging;
using Sydney.Core;

namespace Cfth.Handlers;

public class UrlResourceHandler : ResourceHandlerBase
{
    private readonly IUrlRepository urlRepository;
    private readonly ICodec codec;
    private readonly Uri baseUrl;

    public UrlResourceHandler(
        ILoggerFactory loggerFactory,
        IUrlRepository urlRepository,
        ICodec codec,
        Uri baseUrl) : base(loggerFactory)
    {
        this.urlRepository = urlRepository;
        this.codec = codec;
        this.baseUrl = baseUrl;
    }

    public override async Task<SydneyResponse> CreateAsync(ISydneyRequest request)
    {
        CreateUrlRequest requestBody = await request.DeserializeJsonAsync<CreateUrlRequest>();

        UrlEntity entity = new UrlEntity(requestBody.Url, requestBody.ExpiresAt);
        await this.urlRepository.PutAsync(entity);

        return new SydneyResponse(
            HttpStatusCode.OK,
            new UrlEntityResponse(
                this.codec,
                this.baseUrl,
                entity));
    }

    public override async Task<SydneyResponse> GetAsync(ISydneyRequest request)
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
                this.baseUrl,
                entity));
    }

    public override async Task<SydneyResponse> DeleteAsync(ISydneyRequest request)
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
