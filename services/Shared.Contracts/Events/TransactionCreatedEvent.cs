using System;

namespace Shared.Contracts.Events
{
    public class TransactionCreatedEvent
    {
        public Guid TransactionId { get; set; }

        public Guid UserId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}