using Microsoft.EntityFrameworkCore;
using BudgetTracker.EMIs.Entities;

namespace BudgetTracker.EMIs;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base (options) { }

    public DbSet<MonthlyInstallment> MonthlyInstallments { get; set; }
}

public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
    
    public DbSet<MonthlyInstallment> MonthlyInstallments { get; set; }
}