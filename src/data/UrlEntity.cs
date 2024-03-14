namespace Cfth.Data;

public class UrlEntity
{
    public UrlEntity(Uri url)
    {
        this.Url = url;
    }

    public ulong Id { get; set; }

    public Uri Url { get; set; }
}
