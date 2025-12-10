using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;

namespace BudgetTracker.Finance.Entities;

[Table("banks")]
public class Bank : BaseEntity
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}