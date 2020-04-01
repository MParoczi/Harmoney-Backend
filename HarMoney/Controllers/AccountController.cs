using System;
using System.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HarMoney.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HarMoney.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private const string SECRET_KEY = "harmoney_secret_key";
        public static readonly SymmetricSecurityKey SIGNIN_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));

        private UserManager<User> UserManager { get; }
        private SignInManager<User> SignInManager  { get; }

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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
                var token = GenerateToken(model.Email, user.FirstName, user.LastName, user.Id.ToString());
                return new UserDto()
                {
                    FirstName = user.LastName,
                    LastName = user.FirstName,
                    Email = user.Email,
                    Token = token
                };
            }

            return BadRequest();
        }
        [HttpPost]
        public async Task<int> Logout([FromBody]User userToLogout)
        {
            var user = await UserManager.FindByEmailAsync(userToLogout.Email);
            await UserManager.UpdateSecurityStampAsync(user);
            
            await SignInManager.SignOutAsync();
            return 204;
        }

        [HttpPost]
        public async Task<UserDto> CurrentUser([FromBody]Token jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var result = jwtHandler.ReadToken(jwt.TokenString) as JwtSecurityToken;
            var Email = result.Claims.First(claim => claim.Type == ClaimTypes.UserData).Value;
            var user = await UserManager.FindByEmailAsync(Email);
            return new UserDto(user);
        }
        

        private string GenerateToken(string username, string firstName, string lastName, string id)
        {
            var token = new JwtSecurityToken(
                claims: new Claim[]
                {
                    new Claim(ClaimTypes.UserData, username), 
                    new Claim(ClaimTypes.Name, id), 
                    new Claim(ClaimTypes.Name, lastName), 
                    new Claim(ClaimTypes.Name, firstName), 
                },
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddMinutes(15)).DateTime,
                signingCredentials: new SigningCredentials(SIGNIN_KEY, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}