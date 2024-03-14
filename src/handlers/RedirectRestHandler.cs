using System.Net;
using Cfth.Data;
using Microsoft.Extensions.Logging;
using Sydney.Core;

namespace Cfth.Handlers;

public class RedirectRestHandler : RestHandlerBase
{
    private IUrlRepository urlRepository;
    private ICodec codec;

    public RedirectRestHandler(
        ILoggerFactory loggerFactory,
        IUrlRepository urlRepository,
        ICodec codec) : base(loggerFactory)
    {
        this.urlRepository = urlRepository;
        this.codec = codec;
    }

    public override async Task<SydneyResponse> GetAsync(ISydneyRequest request)
    {
        string encodedId = request.PathParameters["encodedId"];
        if (!this.codec.TryDecode(encodedId, out ulong id))
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid short URL.");
        }

        UrlEntity? entity = await this.urlRepository.GetAsync(id);
        if (entity == null || entity.IsExpired)
        {
            // Return a 404 for expired entities.
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        // Increment usage data
        entity.UsageCounter!.Increment();

        SydneyResponse response = new SydneyResponse(HttpStatusCode.Found);
        response.Headers.Add("Location", entity.Url!.ToString());

        return response;
    }
}
