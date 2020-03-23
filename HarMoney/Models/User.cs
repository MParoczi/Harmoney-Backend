using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HarMoney.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public UserDto()
        {
        }

        public UserDto(User userDetails)
        {
            Id = userDetails.Id;
            Email = userDetails.Email;
            FirstName = userDetails.FirstName;
            LastName = userDetails.LastName;
        }
    }
}