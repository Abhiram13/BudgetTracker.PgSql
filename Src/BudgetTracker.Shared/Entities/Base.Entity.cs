using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

// HACK: This attribute is used to expose "internal" members to specific named Assembly/ Projects
[assembly: InternalsVisibleTo("IntegrationTests.Finance")]

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
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets or sets the date when the entity was last modified.
    /// </summary>
    [Required]
    [Column("updated_at")]
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset UpdatedAt { get; private set; }

    protected void SetModifiedAt()
    {
        DateTimeOffset now =  DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }
    
    protected void SetUpdatedAt()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        UpdatedAt = now;
    }

    internal void SetTimeStamps(DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}