using HarMoney.Models;
using Microsoft.EntityFrameworkCore;

namespace HarMoney.Contexts
{
    public class TransactionContext : DbContext
    {
        TransactionContext() { }
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }

    }
}
