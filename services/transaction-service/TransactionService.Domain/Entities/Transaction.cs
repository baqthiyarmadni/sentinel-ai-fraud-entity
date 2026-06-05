using TransactionService.Domain.Enums;
using TransactionService.Domain.Exceptions;

namespace TransactionService.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string? Currency { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? Location { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction()
    {
        // Private constructor for EF Core or other ORMs
    }

    public Transaction(Guid userId, decimal amount, string currency, string location)
    {
        Validate(userId, amount, currency, location);

        Id = Guid.NewGuid();
        UserId = userId;
        Amount = amount;
        Currency = currency;
        Location = location;
        Status = TransactionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    private void Validate(Guid userId, decimal amount, string currency, string location)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required.");

        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Location is required.");
    }

    public void MarkAsCompleted()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be marked as completed.");
        Status = TransactionStatus.Completed;
    }

    public void MarkAsFailed()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be marked as failed.");
        Status = TransactionStatus.Failed;
    }

    public void MarkAsFlagged()
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Only pending transactions can be marked as flagged.");
        Status = TransactionStatus.Flagged;
    }
}