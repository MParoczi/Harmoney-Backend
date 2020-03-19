using System;
using System.Composition;
using System.Threading.Tasks;
using HarMoney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HarMoney.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager  { get; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public async Task<string> Register([FromBody] UserAuthentication model)
        {
            var user = new User {UserName = model.Email, Email = model.Email};
            
            var result = await UserManager.CreateAsync(user, model.Password);

            return result.ToString();
        }

        public async Task<ActionResult<User>> Login([FromBody]UserAuthentication model)
        {
            // User user = model;
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                return new User();
            }
            else
            {
                return BadRequest();
            }
        }
            
    }
}