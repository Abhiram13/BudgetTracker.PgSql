using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base (options) { }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionsMeta> TransactionsMeta { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
}

public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<TransactionsMeta> TransactionsMeta { get; set; }
}