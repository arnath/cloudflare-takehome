namespace Cfth.Data;

interface IUrlRepository
{
    Task PutAsync(UrlEntity entity);

    Task<UrlEntity?> GetAsync(ulong id);
}
