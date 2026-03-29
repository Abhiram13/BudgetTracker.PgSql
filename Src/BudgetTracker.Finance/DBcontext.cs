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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.Description).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);
            entity.Property(t => t.ActualAmount).HasPrecision(18, 2);
            entity.Property(t => t.CategoryId).IsRequired();
            entity.Property(t => t.Type).IsRequired();
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.Property(t => t.UpdatedAt).IsRequired();
            
            entity.ToTable("transactions", t =>
            {
                t.HasCheckConstraint("CK_Transaction_Amount", "amount >= 0.01 AND amount <= 1000000");
                t.HasCheckConstraint("CK_Transaction_Actual_Amount", "(actual_amount IS NULL) OR (actual_amount >= 0.01 AND actual_amount <= 1000000)");
                t.HasCheckConstraint("CK_Transaction_Description_Regex", "description ~ '^[a-zA-Z0-9# ,]*$'");
                t.HasCheckConstraint("CK_Transactions_Description_MinLength", "LENGTH(TRIM(description)) >= 3");
                t.HasCheckConstraint("CK_Transactions_Type", "type = 1 OR type = 2");
                t.HasCheckConstraint("CK_Transactions_Bank", "(type = 1 AND from_bank IS NOT NULL) OR (type = 2 AND to_bank IS NOT NULL)");
            });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(30);
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.Property(t => t.UpdatedAt).IsRequired();
        });
        
        modelBuilder.Entity<Bank>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(30);
            entity.Property(t => t.CreatedAt).IsRequired();
            entity.Property(t => t.UpdatedAt).IsRequired();
        });
    }
}

[Obsolete(message: "Read-Only DB has issues. So, not using this context")]
public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base (options) { }
    
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bank> Banks { get; set; }
}