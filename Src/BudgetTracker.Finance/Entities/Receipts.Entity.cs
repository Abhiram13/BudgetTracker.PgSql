using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;

namespace BudgetTracker.Finance.Entities;

[Table("receipts")]
public class Receipt : BaseEntity
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public new Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Column("file_name")]
    [JsonPropertyName("file_name")]
    public required string FileName { get; set; }
    
    [Required]
    [Column("object_key")]
    [JsonPropertyName("object_key")]
    public required string ObjectKey { get; set; }
    
    [Required]
    [Column("extension")]
    [JsonPropertyName("extension")]
    public required string Extension { get; set; }
    
    [Required]
    [Column("mime_type")]
    [JsonPropertyName("mime_type")]
    public required string MimeType { get; set; }
    
    [Required]
    [Column("file_size_bytes")]
    [JsonPropertyName("file_size_bytes")]
    public required long FileSize { get; set; }
    
    [Required]
    [Column("md5_hash")]
    [JsonPropertyName("md5_hash")]
    public string? MdHash { get; set; }
}