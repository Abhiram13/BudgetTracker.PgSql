using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Services;

public class BankService
{
    private readonly IBankRepository _bankRepository;

    public BankService(IBankRepository bankRepository)
    {
        _bankRepository = bankRepository;
    }

    public async Task<Bank> InsertBankAsync(InsertBankDto payload)
    {
        Bank bank = Bank.Create(bankName: payload.Name);
        
        return await _bankRepository.InsertOneBankAsync(bank);
    }

    public async Task<List<BankDto>> GetBankListsAsync()
    {
        return await _bankRepository.GetAllBanksAsync();
    }

    public async Task<Bank> GetBankByIdAsync(int id)
    {
        return await _bankRepository.GetBankByIdAsync(id);
    }

    public async Task UpdateBankAsync(InsertBankDto payload, int id)
    {
        await _bankRepository.UpdateBankAsync(id, payload.Name);
    }
}