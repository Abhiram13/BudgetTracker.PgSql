using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

[Table("finance_outbox_events")]
public class FinanceOutboxEvents
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("entity_type")]
    public required string EntityType { get; set; }
    
    [Required]
    [Column("entity_id")]
    public required int EntityId { get; set; }
    
    [Required]
    [Column("event_type")]
    public required string EventType { get; set; }
    
    [Required]
    [Column("payload", TypeName = "jsonb")]
    public required JsonDocument Payload { get; set; }
    
    [Required]
    [Column("status")]
    public required string Status { get; set; }
    
    [Column("retry_count")]
    public int RetryCount { get; set; } = 0;
    
    [Column("error")]
    public string? Error { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }
}