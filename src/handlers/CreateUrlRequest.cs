namespace Cfth.Handlers;

public record class CreateUrlRequest(Uri Url, DateTime? ExpiresAt = null);
