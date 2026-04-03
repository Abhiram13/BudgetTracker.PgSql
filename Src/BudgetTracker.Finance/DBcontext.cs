using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BudgetTracker.Finance;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base (options) { }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionsMeta> TransactionsMeta { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<FinanceOutboxEvents> FinanceOutboxEvents { get; set; }
}

public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<TransactionsMeta> TransactionsMeta { get; set; }
    public DbSet<FinanceOutboxEvents> FinanceOutboxEvents { get; set; }
}

// public class WriteDbContextFactory : IDesignTimeDbContextFactory<WriteDbContext>
// {
//     public WriteDbContext CreateDbContext(string[] args)
//     {
//         // 1. Build configuration to read your settings (appsettings.json / Environment Variables)
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
//         var user = configuration["Postgres:MigrateUsername"];
//         var pass = configuration["Postgres:MigratePassword"];
//         var host = configuration["Postgres:Host"];
//         var db = configuration["Postgres:Database"];
//         var port = configuration["Postgres:MigratePort"];
//
//         string connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
//
//         optionsBuilder.UseNpgsql(connectionString);
//
//         return new WriteDbContext(optionsBuilder.Options);
//     }
// }