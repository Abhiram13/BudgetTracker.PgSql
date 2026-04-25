using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Entities;

[Table("categories")]
public class Category : BaseEntity
{
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; private set; } = string.Empty;
    
    private Category() { }

    public static Category Create(string categoryName)
    {
        Validate(categoryName);
        
        Category category = new Category { Name =  categoryName };
        category.SetModifiedAt();
        
        return category;
    }

    public void Update(string categoryName)
    {
        Validate(categoryName);
        
        Name = categoryName;
        
        SetUpdatedAt();
    }
    
    private static void Validate(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new InvalidPayloadException("Category name is required");
        }
        
        if (categoryName.Length < SharedConstants.LengthConstants.MIN_CATEGORY_LENGTH || categoryName.Length > SharedConstants.LengthConstants.MAX_CATEGORY_LENGTH)
        {
            throw new InvalidPayloadException("Category name exceeds or does not reach required length");
        }

        if (!Regex.IsMatch(categoryName, SharedConstants.ValidationRegex.NAME_PATTERN))
        {
            throw new InvalidPayloadException("Category name contains invalid characters. Only letters, numbers, spaces are allowed");
        }
    }
}