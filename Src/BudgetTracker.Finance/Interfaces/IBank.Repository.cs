using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Interfaces;

public interface IBankRepository
{
    Task<Bank> InsertOneBankAsync(Bank payload);
    Task<List<BankListDto>> GetAllBanksAsync();
    Task<Bank> GetBankByIdAsync(int id);
    Task UpdateBankAsync(Bank payload);
}