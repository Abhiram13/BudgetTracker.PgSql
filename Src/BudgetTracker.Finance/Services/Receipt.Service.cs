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
        Receipt receipt = Receipt.Create(
            extension: payload.Extension,
            fileName: payload.FileName,
            fileSize: payload.FileSize,
            hash: payload.MdHash,
            mimeType: payload.MimeType,
            objectKey: payload.ObjectKey
        );
        
        await _receiptRepository.InsertOneAsync(receipt);
    }
}