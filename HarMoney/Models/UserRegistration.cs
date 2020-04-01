using System.ComponentModel.DataAnnotations;

namespace HarMoney.Models
{
    public class UserRegistration : User
    {
        [Required]
        public string Password { get; set; }
    }
}