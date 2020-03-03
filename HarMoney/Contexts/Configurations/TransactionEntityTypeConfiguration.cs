using System;
using HarMoney.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HarMoney.Contexts.Configurations
{
    internal class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transaction");
            builder.HasKey(prop => prop.Id);
            builder.Property(prop => prop.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(prop => prop.Title).HasColumnName("title").HasMaxLength(30).IsRequired();
            builder.Property(prop => prop.DueDate).HasColumnName("due_date").IsRequired();
            builder.Property(prop => prop.Amount).HasColumnName("amount").IsRequired();
            builder.Property(prop => prop.Frequency).HasColumnName("frequency").IsRequired()
                .HasConversion(
                    f => f.ToString(),
                    f => (Frequency)Enum.Parse(typeof(Frequency), f));
            builder.Property(prop => prop.Direction).HasColumnName("direction").IsRequired()
                .HasConversion(
                    d => d.ToString(),
                    d => (Direction)Enum.Parse(typeof(Direction), d));
        }
    }
}
