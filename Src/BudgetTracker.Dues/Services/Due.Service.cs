using BudgetTracker.Dues.Entities;
using BudgetTracker.Dues.Interfaces;

namespace BudgetTracker.Dues.Services;

public class DueService
{
    private readonly IDueRepository _dueRepository;

    public DueService(IDueRepository dueRepository)
    {
        _dueRepository = dueRepository;
    }

    public async Task InsertOneAsync(Due payload)
    {
        await _dueRepository.InsertDueAsync(payload);
    }
}