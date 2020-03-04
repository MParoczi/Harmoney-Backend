using HarMoney.Contexts.Configurations;
using HarMoney.Models;
using Microsoft.EntityFrameworkCore;

namespace HarMoney.Contexts
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration());
        }
    }
}
