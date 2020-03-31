using System.Threading.Tasks;
using EmailService;
using HarMoney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HarMoney.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager  { get; }
        private ILogger<AccountController> Logger  { get; }
        private IEmailSender EmailSender { get; }

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 ILogger<AccountController> logger,
                                 IEmailSender emailSender)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Logger = logger;
            EmailSender = emailSender;
        }

        public async Task<ActionResult<UserDto>> Register([FromBody] UserRegistration model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if(user == null){
                user = new User {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmationLink =
                        Url.Action("ConfirmEmail", "Account", new {userEmail = user.Email, token = token}, Request.Scheme);
                    var message = new Message(new string[] { user.Email }, "Confirmation letter - Harmoney", confirmationLink);
                    await EmailSender.SendEmailAsync(message);
                }
                return new UserDto(user);
            }
            return BadRequest();
        }

        public async Task<ActionResult<UserDto>> Login([FromBody]UserAuthentication model)
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
        
        public async Task<int> Logout([FromBody]UserDto userToLogout)
        {
            var user = await UserManager.FindByEmailAsync(userToLogout.Email);
            await UserManager.UpdateSecurityStampAsync(user);
            
            await SignInManager.SignOutAsync();
            return 204;
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
            return Redirect("http://localhost:3000/");
        }
    }
}