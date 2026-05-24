using System.Data;
using BudgetTracker.Finance;
using BudgetTracker.Finance.Entities;
using BudgetTracker.Finance.Enums;
using BudgetTracker.Finance.Interfaces;
using BudgetTracker.Finance.Models;
using BudgetTracker.Finance.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Finance.Tests.Transactions;

public class TransactionsUnitTests
{
    private readonly Mock<ITransactionRepository> _repository = new Mock<ITransactionRepository>();
    private readonly Mock<ILogger<TransactionService>> _logger = new Mock<ILogger<TransactionService>>();
    private readonly Mock<TransactionsMetaService>  _transactionsMetaService = new Mock<TransactionsMetaService>();
    private readonly Mock<OutboxService> _outboxService = new Mock<OutboxService>();
    private readonly Mock<WriteDbContext> _writeDbContext = new Mock<WriteDbContext>();
    private readonly Mock<DueService> _dueService = new Mock<DueService>();
    private readonly Mock<IDbContextTransaction> _dbTransaction = new Mock<IDbContextTransaction>();
    private readonly TransactionService _transactionService;    

    public TransactionsUnitTests()
    {
        // _writeDbContext.Setup(x => x.Database.BeginTransactionAsync(default)).ReturnsAsync(_dbTransaction.Object);
        _transactionService = new TransactionService(
            repository: _repository.Object,
            logger: _logger.Object,
            outboxService: _outboxService.Object,
            transactionsMetaService: _transactionsMetaService.Object,
            writeDbContext: _writeDbContext.Object,
            dueService: _dueService.Object
        );
    }

    [Fact]
    public async Task Insert_Transaction_Valid_Async()
    {
        // Arrange
        InsertTransactionDto payload = new InsertTransactionDto
        { 
            ActualAmount = 100, 
            Amount = 100, 
            Description = "Test", 
            DueId = 5, 
            CategoryId = 1, 
            Date = DateOnly.FromDateTime(DateTime.UtcNow), 
            FromBank = 1, 
            ToBank = null, 
            Type = TransactionType.Debit
        };
        
        _dueService.Setup(x => x.IsDueExists(5)).ReturnsAsync(true);
        _repository.Setup(x => x.InsertOneTransactionAsync(It.IsAny<Transaction>())).Returns((Transaction t) => t);

        // Act
        InsertTransactionResponseDto result = await _transactionService.InsertTransactionAsync(payload);

        // Assert
        Assert.NotNull(result);
        _repository.Verify(x => x.InsertOneTransactionAsync(It.IsAny<Transaction>()), Times.Once);
        // _dbTransaction.Verify(x => x.CommitAsync(default), Times.Once);
        // _dbTransaction.Verify(x => x.RollbackAsync(default), Times.Never);
    }
}