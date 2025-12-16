using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Shared.Entities;

public abstract class BaseEntity
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [Column("created_at")]
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? CreatedAt { get; set; }

    [Column("updated_at")]
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly? UpdatedAt { get; set; }
}