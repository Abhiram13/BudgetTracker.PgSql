using System.ComponentModel.DataAnnotations;
using BudgetTracker.Shared.Constants;

namespace BudgetTracker.Finance.Models;

/// <summary>
/// DTO that used to hold the payload from http request when inserting a Bank
/// </summary>
public record InsertBankDto
{
    /// <summary>
    /// Name of the Bank
    /// </summary>
    /// <list type="bullet">
    /// <item>Must contain at least one letter (a-z, A-Z).</item>
    /// <item>Allows only digits, alphabets, comma.</item>
    /// <item>Minimum <c>3</c> characters are required</item>
    /// <item>Maximum <c>25</c> characters are required</item>
    /// </list>
    [Required(ErrorMessage = "Bank name is required")]
    [StringLength(maximumLength: SharedConstants.LengthConstants.MAX_BANK_LENGTH, MinimumLength = SharedConstants.LengthConstants.MIN_BANK_LENGTH, ErrorMessage = "Bank name exceeds or does not reach required length")]
    [RegularExpression(SharedConstants.ValidationRegex.NAME_PATTERN, ErrorMessage = "Only letters, numbers, spaces and # are allowed")]
    public string Name { get; init; } = string.Empty;
}

/// <summary>
/// Holds indiviual bank details
/// </summary>
public record BankDto
{
    /// <summary>
    /// ID of the bank
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Name of the bank
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}