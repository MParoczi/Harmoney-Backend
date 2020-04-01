using System.Threading.Tasks;
using HarMoney.Helpers.Validation;
using HarMoney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HarMoney.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        public UserManager<User> UserManager { get; private set; }
        public SignInManager<User> SignInManager  { get; private set; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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
                        return new UserDto(user);
                    }

                    return BadRequest(new {error = result.Errors});
                }
            }

            return BadRequest(this.ModelState);
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
        
        public async Task<ActionResult> Logout([FromBody]UserDto userToLogout)
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
            
    }
}