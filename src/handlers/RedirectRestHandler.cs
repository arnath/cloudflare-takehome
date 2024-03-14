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

    public override async Task<SydneyResponse> GetAsync(SydneyRequest request)
    {
        string encodedId = request.PathParameters["encodedId"];
        if (!this.codec.TryDecode(encodedId, out ulong id))
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid short URL.");
        }

        UrlEntity? entity = await this.urlRepository.GetAsync(id);
        if (entity == null)
        {
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        SydneyResponse response = new SydneyResponse(HttpStatusCode.TemporaryRedirect);
        response.Headers.Add("Location", entity.OriginalUrl.ToString());

        return response;
    }
}
