using System.Threading.Tasks;
using System;
using HarMoney.Helpers.Validation;
using EmailService;
using HarMoney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HarMoney.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        public UserManager<User> UserManager { get; private set; }
        public SignInManager<User> SignInManager { get; private set; }
        public IEmailSender EmailSender { get; private set; }

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Register([FromBody] UserRegistration model)
        {
            if (model.Email == null)
            {
                return BadRequest(new {error = "The Email field is required."});
            }

            PasswordSpecialCharacterValidator validator = new PasswordSpecialCharacterValidator();
            if (!validator.IsOk(model.Password))
            {
                return BadRequest(validator.ErrorMessage);
            }

            if (this.ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName
                    };

                    IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        string token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                        string confirmationLink =
                            Url.Action("ConfirmEmail", "Account", new {userEmail = user.Email, token = token},
                                Request.Scheme);
                        
                        string emailContent = EmailSender.CreateEmailContent(user.FirstName, confirmationLink);
                        Message message = new Message(new string[] { user.Email }, "Confirmation letter - Harmoney", emailContent);
                        
                        await EmailSender.SendEmailAsync(message);
                        return new UserDto(user);
                    }

                    return BadRequest(new {error = result.Errors});
                }
            }

            return BadRequest(this.ModelState);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Login([FromBody] UserAuthentication model)
        {
            User user = await UserManager.FindByEmailAsync(model.Email);
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                return new UserDto(user);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Logout([FromBody] UserDto userToLogout)
        {
            if (this.ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(userToLogout.Email);
                await UserManager.UpdateSecurityStampAsync(user);

                await SignInManager.SignOutAsync();
                return NoContent();
            }

            return BadRequest(this.ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userEmail, string token)
        {
            if (userEmail == null || token == null)
            {
                return BadRequest();
            }

            User user = await UserManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return BadRequest();
            }

            await UserManager.ConfirmEmailAsync(user, token);
            return Redirect(Environment.GetEnvironmentVariable("HARMONEY_FRONTEND"));
        }
    }
}