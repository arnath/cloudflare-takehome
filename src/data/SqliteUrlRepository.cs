using Microsoft.EntityFrameworkCore;

namespace Cfth.Data;

public class SqliteUrlRepository : DbContext, IUrlRepository
{
    private readonly string dbPath;

    public SqliteUrlRepository()
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        this.dbPath = Path.Join(folderPath, "cfth.db");
    }

    public DbSet<UrlEntity> Urls { get; set; }

    public async ValueTask PutAsync(UrlEntity entity)
    {
        // This sets the Id field in the entity object.
        this.Urls.Add(entity);
        await this.SaveChangesAsync();
    }

    public ValueTask<UrlEntity?> GetAsync(ulong id)
    {
        return this.Urls.FindAsync(id);
    }
    
    public async ValueTask DeleteAsync(ulong id)
    {
        UrlEntity entity = new UrlEntity(id);
        this.Urls.Remove(entity);
        await this.SaveChangesAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={this.dbPath}");
}
