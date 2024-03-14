namespace Cfth.Handlers;

public record class CreateUrlRequest(Uri Url, DateTime? Expiration = null);
