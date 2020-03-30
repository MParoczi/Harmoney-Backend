using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HarMoney.Contexts.Configurations;
using HarMoney.Models;
using Marques.EFCore.SnakeCase;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HarMoney.Contexts
{
    public class IdentityAppContext: IdentityDbContext<User, AppRole, int>
    {
        public IdentityAppContext(DbContextOptions<IdentityAppContext> options) : base(options)
        {

        }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new TransactionEntityTypeConfiguration());
            builder.ToSnakeCase();
            // builder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .UseNpgsql(Environment.GetEnvironmentVariable("HARMONEY_CONNECTION"));
    }
}
