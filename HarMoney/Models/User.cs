using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarMoney.Models
{
    public class User
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
