using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTracker.Shared.Entities;

/// <summary>
/// Serves as the base class for all domain entities, providing common properties such as ID, Created at and Updated at.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    /// <remarks>
    /// This property is immutable after initialization (<c>init</c>).
    /// </remarks>
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the date when the entity was first created.
    /// </summary>
    /// <remarks>
    /// This property is immutable after initialization (<c>init</c>).
    /// </remarks>
    [Required]
    [Column("created_at")]
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the date when the entity was last modified.
    /// </summary>
    [Required]
    [Column("updated_at")]
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateOnly UpdatedAt { get; set; }
}