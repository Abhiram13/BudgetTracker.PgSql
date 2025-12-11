namespace BudgetTracker.Finance.Models;

public record class InsertCategoryDto
{
    public string Name { get; set; } = string.Empty;
}