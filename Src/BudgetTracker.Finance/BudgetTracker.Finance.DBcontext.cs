using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base (options) { }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
}

[Obsolete(message: "Read-Only DB has issues. So, not using this context")]
public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
}