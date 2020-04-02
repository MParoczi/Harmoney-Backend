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
    /// <summary>
    /// Controller class for the Account management. Handles the HTTPRequest from the frontend.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Provides the APIs for managing user in a persistence store.
        /// </summary>
        public UserManager<User> UserManager { get; private set; }
        
        /// <summary>
        /// Provides the APIs for user sign in.
        /// </summary>
        public SignInManager<User> SignInManager { get; private set; }
        
        /// <summary>
        /// Provides the APIs for sending automatized e-mails. In this case used for registration confirmation.
        /// </summary>
        public IEmailSender EmailSender { get; private set; }
        
        private static readonly string SECRET_KEY = Environment.GetEnvironmentVariable("HARMONEY_SECRET_KEY");
        public static readonly SymmetricSecurityKey SIGNIN_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));

        /// <summary>
        /// Constructor for the Account controller. Parameters are injected via dependency injection.
        /// </summary>
        /// <param name="userManager">The API for managing user in a persistence store</param>
        /// <param name="signInManager">The API for user sign in</param>
        /// <param name="emailSender">The API for automatized e-mail handling</param>
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
        }

        /// <summary>
        /// Controls the user registration. It can handle POST requests.
        /// </summary>
        /// <param name="model">The model that represents the user's details and credentials for registration</param>
        /// <returns>Returns either Bad Request in case of unsuccessful registration or the newly created User if the registration succeeded</returns>
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

        /// <summary>
        /// Controls the user login. It can handle POST requests.
        /// </summary>
        /// <param name="model">The model that represents the user's details and credentials for login</param>
        /// <returns>Returns either Bad Request in case of unsuccessful login or the found User if the login succeeded</returns>
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

        /// <summary>
        /// Controls the user logout. It can handle POST requests.
        /// </summary>
        /// <param name="userToLogout">The model that represents the user's details and credentials for login</param>
        /// <returns>It returns No content response in case of success otherwise Bad Request</returns>
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

        /// <summary>
        /// Controls the registration confirmation. It redirects to the HarMoney frontend if the confirmation was successful.
        /// </summary>
        /// <param name="userEmail">The user's e-mail address where the service has sent the confirmation letter</param>
        /// <param name="token">The token that validates the registration</param>
        /// <returns>If the confirmation was successful, the controller will redirect to the HarMoney frontend</returns>
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