namespace Cfth.Data;

public interface IUrlRepository
{
    ValueTask PutAsync(UrlEntity entity);

    ValueTask<UrlEntity?> GetAsync(ulong id);

    ValueTask DeleteAsync(ulong id);
}
