using System.Text.Json.Serialization;
using Base62;

namespace Cfth.Data;

public class UrlEntity
{
    public UrlEntity(Uri originalUrl)
    {
        this.OriginalUrl = originalUrl;
    }

    [JsonIgnore]
    public ulong Id { get; set; }

    public Uri OriginalUrl { get; set; }
}
