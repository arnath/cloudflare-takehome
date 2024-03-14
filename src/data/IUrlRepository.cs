namespace Cfth.Data;

public interface IUrlRepository
{
    Task PutAsync(UrlEntity entity);

    Task<UrlEntity?> GetAsync(ulong id);

    Task DeleteAsync(ulong id);
}
