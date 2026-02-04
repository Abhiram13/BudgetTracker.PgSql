using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Finance.Repository;

public class BankRepository : IBankRepository
{
    private readonly WriteDbContext _writeDbContext;

    public BankRepository(WriteDbContext write)
    {
        _writeDbContext = write;
    }

    public async Task<List<BankListDto>> GetAllBanksAsync()
    {
        List<BankListDto> list = await _writeDbContext.Banks
            .Select(b => new BankListDto { Id = b.Id, Name = b.Name })
            .ToListAsync();

        return list;
    }

    public async Task<Bank> GetBankByIdAsync(int id)
    {
        Bank? bank = await _writeDbContext.Banks.FirstOrDefaultAsync(b => b.Id == id);
        
        if (bank == null) throw new BadHttpRequestException($"Bank with id {id} not found");
        
        return bank;
    }

    public async Task UpdateBankAsync(Bank payload)
    {
        Bank bank = await GetBankByIdAsync(payload.Id);
        bank.Name = payload.Name;
        bank.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task<Bank> InsertOneBankAsync(Bank payload)
    {
        await _writeDbContext.Banks.AddAsync(payload);
        await _writeDbContext.SaveChangesAsync();
        return payload;
    }
}