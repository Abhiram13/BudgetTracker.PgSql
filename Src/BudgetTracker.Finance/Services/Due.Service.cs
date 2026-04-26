using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Entities;

namespace BudgetTracker.Finance.Services;

public sealed class DueService
{
    private readonly IDueRepository _dueRepository;

    public DueService(IDueRepository dueRepository)
    {
        _dueRepository = dueRepository;
    }

    public async Task InsertOneAsync(InsertDueDto payload)
    {
        Due due = Due.Create(
            creditor: payload.Creditor,
            description: payload.Description, 
            comments: payload.Comments, 
            debtor: payload.Debtor, 
            remarks: payload.Remarks, 
            startDate: payload.StartDate, 
            title: payload.Title, 
            totalAmount: payload.TotalAmount
        );
        
        await _dueRepository.InsertOneAsync(due);
    }

    public async Task<List<DueListDetailsDto>> GetListAsync()
    {
        return await _dueRepository.GetListOfDuesAsync();
    }

    public async Task<bool> IsDueExists(int id)
    {
        return await _dueRepository.IsDueExistAsync(id);
    }
}