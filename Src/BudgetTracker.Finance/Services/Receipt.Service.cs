using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;

namespace BudgetTracker.Finance.Services;

public class ReceiptService
{
    private readonly ILogger<ReceiptService> _logger;
    private readonly IReceiptRepository _receiptRepository;

    public ReceiptService(ILogger<ReceiptService> logger, IReceiptRepository receiptRepository)
    {
        _logger = logger;
        _receiptRepository = receiptRepository;
    }

    public async Task AddReceiptAsync(InsertReceiptDto payload)
    {
        Receipt receipt = new Receipt
        {
            Extension = payload.Extension,
            ObjectKey = payload.ObjectKey,
            FileName = payload.FileName,
            MimeType = payload.MimeType,
            FileSize = payload.FileSize,
            MdHash = payload.MdHash
        };
        
        await _receiptRepository.InsertOneAsync(receipt);
    }
}