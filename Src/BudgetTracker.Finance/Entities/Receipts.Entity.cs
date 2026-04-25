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
    public new Guid Id { get; private set; } = Guid.NewGuid();
    
    [Required]
    [Column("file_name")]
    public string FileName { get; private set; }
    
    [Required]
    [Column("object_key")]
    public string ObjectKey { get; private set; }
    
    [Required]
    [Column("extension")]
    public string Extension { get; private set; }
    
    [Required]
    [Column("mime_type")]
    public string MimeType { get; private set; }
    
    [Required]
    [Column("file_size_bytes")]
    public long FileSize { get; private set; }
    
    [Required]
    [Column("md5_hash")]
    public string? MdHash { get; private set; }
    
    private Receipt() { }

    public static Receipt Create(string fileName, string objectKey, string extension, string mimeType, long fileSize, string? hash)
    {
        // TODO: Validations

        Receipt receipt = new Receipt
        {
            Extension =  extension,
            FileName =  fileName,
            FileSize = fileSize,
            MdHash = hash,
            MimeType = mimeType,
            ObjectKey =  objectKey,
        };
        
        return receipt;
    }
}