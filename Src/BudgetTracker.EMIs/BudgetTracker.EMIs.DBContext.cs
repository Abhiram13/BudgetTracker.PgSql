using Microsoft.EntityFrameworkCore;
using BudgetTracker.EMIs.Entities;

namespace BudgetTracker.EMIs;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base (options) { }

    public DbSet<MonthlyInstallment> MonthlyInstallments { get; set; }
}

[Obsolete(message: "Read-Only DB has issues. So, not using this context")]
public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
    
    public DbSet<MonthlyInstallment> MonthlyInstallments { get; set; }
}