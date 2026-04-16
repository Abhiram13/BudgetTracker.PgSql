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

    public async Task<Bank> InsertBankAsync(Bank payload)
    {
        Bank bank = await _bankRepository.InsertOneBankAsync(payload);
        return bank;
    }

    public async Task<List<BankDto>> GetBankListsAsync()
    {
        return await _bankRepository.GetAllBanksAsync();
    }

    public async Task<Bank> GetBankByIdAsync(int id)
    {
        return await _bankRepository.GetBankByIdAsync(id);
    }

    public async Task UpdateBankAsync(Bank payload)
    {
        await _bankRepository.UpdateBankAsync(payload);
    }
}