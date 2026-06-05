using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount).HasPrecision(18, 2);

            builder.Property(t => t.Currency).HasMaxLength(10).IsRequired();

            builder.Property(t => t.Location).HasMaxLength(100).IsRequired();

            builder.Property(t => t.CreatedAt).IsRequired();

            builder.HasIndex(t => t.Id);

            builder.HasIndex(t => t. CreatedAt);

            builder.HasIndex(t => t.Status);

        }
    }
}
