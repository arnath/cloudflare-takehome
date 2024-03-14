namespace Cfth.Data;

public class InMemoryUrlRepository : IUrlRepository
{
    private ulong autoIncrementId = 0;

    private readonly Dictionary<ulong, UrlEntity> entities;
    
    public InMemoryUrlRepository()
    {
        this.entities = new Dictionary<ulong, UrlEntity>();
    }

    public Task PutAsync(UrlEntity entity)
    {
        ulong id = Interlocked.Increment(ref autoIncrementId);
        entity.Id = id;
        this.entities[id] = entity;

        return Task.CompletedTask;
    }

    public Task<UrlEntity?> GetAsync(ulong id)
    {
        this.entities.TryGetValue(id, out UrlEntity? value);
        return Task.FromResult(value);
    }

    public Task DeleteAsync(ulong id)
    {
        this.entities.Remove(id);

        return Task.CompletedTask;
    }
}
