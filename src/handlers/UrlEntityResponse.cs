using Cfth.Data;

namespace Cfth.Handlers;

public class UrlEntityResponse
{
    public UrlEntityResponse(ICodec codec, Uri baseUrl, UrlEntity entity)
    {
        this.Id = entity.Id;
        this.OriginalUrl = entity.Url;
        this.ShortenedUrl = new Uri(baseUrl, codec.Encode(this.Id));
    }

    public ulong Id { get; set; }

    public Uri OriginalUrl { get; set; }

    public Uri ShortenedUrl { get; set; }
}
