using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;
using TransactionService.Application.DTOs;

namespace TransactionService.Application.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);

        Task<Transaction?> GetByIdAsync(Guid id);

        Task<List<Transaction>> GetAllAsync();

        Task SaveChangesAsync();

        Task<PagedResult<Transaction>> GetPagedAsync(TransactionQueryParameters parameters);
    }
}
