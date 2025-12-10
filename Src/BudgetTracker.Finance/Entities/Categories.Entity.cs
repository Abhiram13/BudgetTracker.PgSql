using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;

namespace BudgetTracker.Finance.Entities;

[Table("categories")]
public class Category : BaseEntity
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}