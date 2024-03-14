namespace Cfth.Data;

public class UrlEntity
{
    public UrlEntity(Uri url, DateTime? expiresAt = null, UsageCounter? usageCounter = null)
    {
        this.Url = url;
        this.ExpiresAt = expiresAt;
        this.UsageCounter = usageCounter ?? new UsageCounter();
    }

    public ulong Id { get; set; }

    public Uri Url { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public UsageCounter UsageCounter { get; set; }

    public bool IsExpired => this.ExpiresAt != null && DateTime.UtcNow > this.ExpiresAt;
}
