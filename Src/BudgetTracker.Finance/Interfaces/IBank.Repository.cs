using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface IBankRepository
{
    Task<Bank> InsertOneBankAsync(Bank payload);
    Task<List<BankDto>> GetAllBanksAsync();
    Task<Bank> GetBankByIdAsync(int id);
    Task UpdateBankAsync(Bank payload);
}