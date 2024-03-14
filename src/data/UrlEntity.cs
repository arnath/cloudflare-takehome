namespace Cfth.Data;

public class UrlEntity
{
    public UrlEntity(Uri url, DateTime? expiresAt = null, UsageCounter? usageCounter = null)
    {
        this.Url = url;
        this.ExpiresAt = expiresAt;
        this.UsageCounter = usageCounter ?? new UsageCounter();
    }

    /// <summary>
    /// This constructor is used by the EF Core repositories to create an instance to delete.
    /// It should not be used normally.
    /// </summary>
    internal UrlEntity(ulong id)
    {
        this.Id = id;
    }

    public ulong Id { get; set; }

    public Uri? Url { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public UsageCounter? UsageCounter { get; set; }

    public bool IsExpired => this.ExpiresAt != null && DateTime.UtcNow > this.ExpiresAt;
}
