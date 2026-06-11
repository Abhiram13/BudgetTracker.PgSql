using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Shared.Exceptions;

namespace BudgetTracker.Finance.Services;

public class DueService
{
    private readonly IDueRepository _dueRepository;
    private ILogger<DueService> _logger;

    public DueService(IDueRepository dueRepository, ILogger<DueService> logger)
    {
        _dueRepository = dueRepository;
        _logger = logger;
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

    /// <summary>
    /// Checks and returns if Due exists with given <paramref name="id"/>
    /// </summary>
    /// <param name="id">Due Id to verify</param>
    /// <returns>True if due exists or False</returns>
    /// <exception cref="InvalidPayloadException">When a Due does not exists with given <paramref name="id"/></exception>
    public async Task<bool> IsDueExists(int? id)
    {
        bool isDueExist = false;
        
        if (id is int dueId)
        {
            isDueExist = await _dueRepository.IsDueExistAsync(dueId);

            if (!isDueExist)
            {
                throw new InvalidPayloadException($"Due id ({dueId}) is invalid");
            }
        }
        
        return isDueExist;
    }
}