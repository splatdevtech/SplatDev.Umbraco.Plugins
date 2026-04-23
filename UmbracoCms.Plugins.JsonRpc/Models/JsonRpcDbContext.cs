using Microsoft.EntityFrameworkCore;

namespace UmbracoCms.Plugins.JsonRpc.Models;

public class JsonRpcDbContext : DbContext
{
    public JsonRpcDbContext(DbContextOptions<JsonRpcDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<ApiLog> ApiLogs => Set<ApiLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("jsonrpc");
        base.OnModelCreating(modelBuilder);
    }
}
