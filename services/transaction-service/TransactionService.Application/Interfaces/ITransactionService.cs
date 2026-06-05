using TransactionService.Application.DTOs;

namespace TransactionService.Application.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponse> CreateTransactionAsync(
        CreateTransactionRequest request);

    Task<TransactionResponse> GetByIdAsync(Guid id);

    Task<List<TransactionResponse>> GetAllAsync();

    Task<PagedResponse<TransactionResponse>> GetPagedAsync(TransactionQueryParameters parameters);
}