using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using HarMoney.Helpers.Validation;
using EmailService;
using HarMoney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HarMoney.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        public UserManager<User> UserManager { get; }
        public SignInManager<User> SignInManager { get; }
        public IEmailSender EmailSender { get; }

        private static readonly string SECRET_KEY = Environment.GetEnvironmentVariable("HARMONEY_SECRET_KEY");
        public static readonly SymmetricSecurityKey SIGNIN_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));

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
                var token = GenerateToken(model.Email, user.FirstName, user.LastName, user.Id.ToString());
                UserDto userDto = new UserDto(user);
                userDto.Token = token;
                return userDto;
            }

            return BadRequest();
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

        [HttpPost]
        public async Task<UserDto> CurrentUser([FromBody]Token jwt)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var result = jwtHandler.ReadToken(jwt.TokenString) as JwtSecurityToken;
            var Email = result.Claims.First(claim => claim.Type == ClaimTypes.UserData).Value;
            var user = await UserManager.FindByEmailAsync(Email);
            UserDto userDto = new UserDto(user);
            userDto.Token = jwt.TokenString;
            return userDto;
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