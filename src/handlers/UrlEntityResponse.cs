using Cfth.Data;

namespace Cfth.Handlers;

public class UrlEntityResponse
{
    public UrlEntityResponse(ICodec codec, Uri baseUrl, UrlEntity entity)
    {
        this.Id = entity.Id;
        this.OriginalUrl = entity.Url;
        this.ShortenedUrl = new Uri(baseUrl, codec.Encode(this.Id));
        this.ExpiresAt = entity.ExpiresAt;
    }

    public ulong Id { get; }

    public Uri OriginalUrl { get; }

    public Uri ShortenedUrl { get; }

    public DateTime? ExpiresAt { get; }
}
