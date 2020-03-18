using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public DbSet<HarMoney.Models.User> User { get; set; }

    }
}
