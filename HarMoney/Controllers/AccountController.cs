using System;
using System.Composition;
using System.Threading.Tasks;
using HarMoney.Models;
using Microsoft.AspNetCore.Authorization;
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

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Logger = logger;
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
            
    }
}