using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BudgetTracker.Finance;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public abstract class BaseDbContext<T> : DbContext where T : DbContext
{
    public BaseDbContext(DbContextOptions<T> options) : base(options) { }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionsMeta> TransactionsMeta { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<FinanceOutboxEvents> FinanceOutboxEvents { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
}

public class WriteDbContext : BaseDbContext<WriteDbContext>
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base (options) { }
}

public class ReadDbContext : BaseDbContext<ReadDbContext>
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
}

public class MigrateDbContext : BaseDbContext<MigrateDbContext>
{
    public MigrateDbContext(DbContextOptions<MigrateDbContext> options) : base (options) { }
}

// public class WriteDbContextFactory : IDesignTimeDbContextFactory<WriteDbContext>
// {
//     public WriteDbContext CreateDbContext(string[] args)
//     {
//         // BUG: Hardcoding of the strings is working in the connection string. But loading from configuration is not.
//         // seems the directory is not right
//         IConfigurationRoot configuration = new ConfigurationBuilder()
//             .SetBasePath(Directory.GetCurrentDirectory())
//             .AddJsonFile("appsettings.json", optional: true)
//             .AddEnvironmentVariables() 
//             .Build();
//
//         DbContextOptionsBuilder<WriteDbContext> optionsBuilder = new DbContextOptionsBuilder<WriteDbContext>();
//
//         // 2. Extract your migration-specific credentials
//         // You can hardcode this temporarily to test, or pull from config:
//         var user = configuration["Postgres:Migrate:Username"];
//         var pass = configuration["Postgres:Migrate:Password"];
//         var host = configuration["Postgres:Migrate:Host"];
//         var db = configuration["Postgres:Migrate:Database"];
//         var port = configuration["Postgres:Migrate:Port"];
//
//         string connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
//
//         optionsBuilder.UseNpgsql(connectionString);
//
//         return new WriteDbContext(optionsBuilder.Options);
//     }
// }