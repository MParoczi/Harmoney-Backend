using System;
using System.Threading.Tasks;
using HarMoney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HarMoney.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager  { get; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public async Task<string> Register()
        {
            User user = await UserManager.FindByNameAsync("TestUser");
            IdentityResult result = new IdentityResult();
            if (user == null)
            {
                user = new User();
                user.UserName = "TestUser";
                user.Email = "test@test.hu";
                user.FirstName = "Teszt";
                user.LastName = "Elek";

                result = await UserManager.CreateAsync(user, "Test@1234");

            }
            return result.ToString();
        }

        public async Task<String> Login()
        {
            var result = await SignInManager.PasswordSignInAsync("TestUser", "Test@1234", false, false);
            if (result.Succeeded)
            {
                return result.ToString();
            }
            else
            {
                return result.ToString();
            }
        }
            
    }
}