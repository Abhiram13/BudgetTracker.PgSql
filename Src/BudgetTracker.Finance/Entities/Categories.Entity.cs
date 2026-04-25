using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BudgetTracker.Shared.Entities;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.Entities;

[Table("categories")]
public class Category : BaseEntity
{
    [Column("name")]
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_CATEGORY_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_CATEGORY_LENGTH, ErrorMessage = "Category name exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces are allowed")]
    public string Name { get; private set; } = string.Empty;
    
    private Category() { }

    public static Category Create(string categoryName)
    {
        Category category = new Category { Name =  categoryName };
        
        category.SetModifiedAt();
        
        return category;
    }

    public void Update(string categoryName)
    {
        Name = categoryName;
        
        SetUpdatedAt();
    }
}