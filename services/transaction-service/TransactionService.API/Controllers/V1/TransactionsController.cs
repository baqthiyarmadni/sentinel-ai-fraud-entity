using Microsoft.AspNetCore.Mvc;
using TransactionService.API.Responses;
using TransactionService.Application.DTOs;
using TransactionService.Application.Interfaces;

namespace TransactionService.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            var result = await _transactionService.CreateTransactionAsync(request);

            var response = new ApiResponse<TransactionResponse>
            {
                Success = true,
                Message = "Transaction created successfully",
                Data = result
            };

            return CreatedAtAction(nameof(CreateTransaction), new { id = result.TransactionId }, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _transactionService.GetAllAsync();
            
            return Ok(
                new ApiResponse<List<TransactionResponse>>
                {
                    Success = true,
                    Message = "Transactions retrieved successfully",
                    Data = result
                }
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);

            if (transaction == null)
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Transaction not found",
                    }
                );

            return Ok(
                    new ApiResponse<TransactionResponse>
                    {
                        Success = true,
                        Message = "Transaction retrieved successfully",
                        Data= transaction
                    }
                );
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] TransactionQueryParameters parameters)
        {
            var result = await _transactionService.GetPagedAsync(parameters);

            return Ok(
                new ApiResponse<PagedResponse<TransactionResponse>>
                {
                    Success = true,
                    Message = "Transactions retrieved",
                    Data = result
                });
        }

    }
}
