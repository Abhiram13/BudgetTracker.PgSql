using Microsoft.EntityFrameworkCore;
using BudgetTracker.Dues.Entities;

namespace BudgetTracker.Dues;

public class WriteDBContext : DbContext
{
    public WriteDBContext(DbContextOptions<WriteDBContext> options) : base(options) { }

    public DbSet<Due> Dues { get; set; }
}

public class ReadDBContext : DbContext
{
    public ReadDBContext(DbContextOptions<WriteDBContext> options) : base(options) { }

    public DbSet<Due> Dues { get; set; }
}