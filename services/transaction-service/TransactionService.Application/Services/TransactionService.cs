using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Application.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace TransactionService.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionService> _logger;
    private readonly IEventProducer _eventProducer;

    public TransactionService(
        ITransactionRepository transactionRepository, 
        IMapper mapper, 
        ILogger<TransactionService> logger,
        IEventProducer eventProducer)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _logger = logger;
        _eventProducer = eventProducer;
    }
    public async Task<TransactionResponse> CreateTransactionAsync(
        CreateTransactionRequest request)
    {
        // Create domain entity
        var transaction = new Transaction(
            request.UserId,
            request.Amount,
            request.Currency,
            request.Location);

        _logger.LogInformation("Creating transaction for UserId {UserId}", request.UserId);

        await _transactionRepository.AddAsync(transaction);

        await _transactionRepository.SaveChangesAsync();

        _logger.LogInformation("Transaction created successfully {TransactionId}", transaction.Id);

        var transactionCreatedEvent = new TransactionCreatedEvent
        {
            TransactionId = transaction.Id,
            UserId = transaction.UserId,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Location = transaction.Location,
            CreatedAt = transaction.CreatedAt
        };

        // Map entity to response DTO
        return _mapper.Map<TransactionResponse>(transaction);
    }

    public async Task<TransactionResponse> GetByIdAsync(Guid id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            throw new KeyNotFoundException($"Transaction with ID {id} not found.");
        }
        return _mapper.Map<TransactionResponse>(transaction);
    }

    public async Task<List<TransactionResponse>> GetAllAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();
        return _mapper.Map<List<TransactionResponse>>(transactions);
    }

    public async Task<PagedResponse<TransactionResponse>> GetPagedAsync(TransactionQueryParameters parameters)
    {
        var result = await _transactionRepository.GetPagedAsync(parameters);

        var items = _mapper.Map<List<TransactionResponse>>(result.Items);

        return new PagedResponse<TransactionResponse>
        {
            Items = items,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalRecords = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)parameters.PageSize)
        };
    }
}