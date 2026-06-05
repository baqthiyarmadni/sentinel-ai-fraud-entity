using Microsoft.EntityFrameworkCore;
using TransactionService.Application.Repositories;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Database;
using TransactionService.Application.DTOs;
using System.Linq.Expressions;

namespace TransactionService.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Transaction>> GetAllAsync()
    {
        return await _context.Transactions.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<Transaction>> GetPagedAsync(TransactionQueryParameters parameters)
    {
        IQueryable<Transaction> query = _context.Transactions.AsQueryable();

        if(!string.IsNullOrWhiteSpace(parameters.Status))
        {
            query = query.Where(x => x.Status.ToString() == parameters.Status);
        }

        if(!string.IsNullOrWhiteSpace(parameters.Currency))
        {
            query = query.Where(x => x.Currency == parameters.Currency);
        }

        var sortBy = parameters.SortBy?.Trim();
        var asc = string.Equals(parameters.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        Expression<Func<Transaction, object>> orderSelector = sortBy is not null && sortBy.Equals("transactionid", StringComparison.OrdinalIgnoreCase) ? (Expression<Func<Transaction, object>>)(x => x.Id)
            : sortBy is not null && sortBy.Equals("amount", StringComparison.OrdinalIgnoreCase) ? (x => x.Amount)
            : sortBy is not null && sortBy.Equals("currency", StringComparison.OrdinalIgnoreCase) ? (x => x.Currency!)
            : sortBy is not null && sortBy.Equals("status", StringComparison.OrdinalIgnoreCase) ? (x => x.Status)
            : (x => x.CreatedAt);

        query = asc ? query.OrderBy(orderSelector) : query.OrderByDescending(orderSelector);

        var transactions = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var totalCount = await query.CountAsync();

        return new PagedResult<Transaction>
        {
            Items = transactions,
            TotalCount = totalCount
        };
    }
}