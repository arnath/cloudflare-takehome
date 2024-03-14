namespace Cfth.Data;

public class InMemoryUrlRepository : IUrlRepository
{
    private ulong autoIncrementId = 0;

    private readonly Dictionary<ulong, UrlEntity> entities;
    
    public InMemoryUrlRepository()
    {
        this.entities = new Dictionary<ulong, UrlEntity>();
    }

    public ValueTask PutAsync(UrlEntity entity)
    {
        ulong id = Interlocked.Increment(ref autoIncrementId);
        entity.Id = id;
        this.entities[id] = entity;

        return ValueTask.CompletedTask;
    }

    public ValueTask<UrlEntity?> GetAsync(ulong id)
    {
        this.entities.TryGetValue(id, out UrlEntity? value);
        return new ValueTask<UrlEntity?>(value);
    }

    public ValueTask DeleteAsync(ulong id)
    {
        this.entities.Remove(id);

        return ValueTask.CompletedTask;
    }
}
