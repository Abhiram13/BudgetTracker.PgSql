using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Shared.Entities;
using BudgetTracker.EMIs.Enums;

namespace BudgetTracker.EMIs.Entities;

[Table("monthly_installments")]
public class MonthlyInstallment : BaseEntity
{
    [Column("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [Column("installments")]
    [JsonPropertyName("installments")]
    [Comment(comment: "Total number of monthly installments")]
    public int Installments { get; set; }

    [Column("completed")]
    [JsonPropertyName("completed")]
    [Comment(comment: "Leftover number of monthly installments")]
    public int Completed { get; set; }

    [Column("principle_amount")]
    [JsonPropertyName("principle_amount")]
    [Comment(comment: "Total amount to be paid")]
    public decimal PrincipleAmount { get; set; }

    [Column("installment_amount")]
    [JsonPropertyName("installment_amount")]
    [Comment(comment: "Amount to be paid at each installment")]
    public decimal InstallmentAmount { get; set; }

    [Column("start_date")]
    [JsonPropertyName("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    [JsonPropertyName("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("status")]
    [JsonPropertyName("status")]
    public InstallmentType Status { get; set; } = InstallmentType.Active;

    [Column("remarks")]
    [JsonPropertyName("remarks")]
    [Comment(comment: "Additional info apert from description to describe the payment of the Installment")]
    public string Remarks { get; set; } = string.Empty;

    [Column("comments")]
    [JsonPropertyName("comments")]
    [Comment(comment: "Comments to describe each update action on Installment")]
    public string Comments { get; set; } = string.Empty;
}