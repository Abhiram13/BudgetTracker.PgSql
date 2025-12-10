using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;

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
}