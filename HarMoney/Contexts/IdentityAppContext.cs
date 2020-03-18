using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HarMoney.Contexts.Configurations;
using HarMoney.Models;
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
            // builder.ApplyConfiguration(new UserEntityTypeConfiguration());
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                entityType.SetTableName(ConvertToSnakeCase(entityType.GetTableName()));
                foreach (var property in entityType.GetProperties())
                {
                    property.SetColumnName(ConvertToSnakeCase(property.Name));
                    foreach (var fk in entityType.FindForeignKeys(property))
                    {
                        fk.SetConstraintName(ConvertToSnakeCase(fk.GetConstraintName()));
                    }
                   
                }
            }
            
        }
        
        private string ConvertToSnakeCase(String name)
        {
            var result = Regex.Replace(name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

            return result.ToLower();
        }
    }
}
