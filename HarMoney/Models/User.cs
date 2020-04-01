using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HarMoney.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be 1-50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ffáÁéÉíÍóÓőŐúÚűŰ., '\\s-]{1,}$",
            ErrorMessage = "Only letters ' - . , and spaces are allowed")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Last name must be 1-20 characters long.")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ffáÁéÉíÍóÓőŐúÚűŰ., '\\s-]{1,}$",
            ErrorMessage = "Only letters ' - . , and spaces are allowed.")]
        public string LastName { get; set; }
    }

    public class UserDto
    {   [Required]
        public int? Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be 1-50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ffáÁéÉíÍóÓőŐúÚűŰ., '\\s-]{1,}$",
            ErrorMessage = "Only letters ' - . , and spaces are allowed")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Last name must be 1-20 characters long.")]
        [RegularExpression(@"^[a-zA-Z\u00C0-\u00D6\u00D8-\u00f6\u00f8-\u00ffáÁéÉíÍóÓőŐúÚűŰ., '\\s-]{1,}$",
            ErrorMessage = "Only letters ' - . , and spaces are allowed")]
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