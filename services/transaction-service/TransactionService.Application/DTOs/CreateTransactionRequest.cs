using System.ComponentModel.DataAnnotations;

namespace TransactionService.Application.DTOs;

public class CreateTransactionRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Range(1, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(10)]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Location { get; set; } = string.Empty;
}