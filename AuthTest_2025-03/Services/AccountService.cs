using AuthTest_2025_03.Identity.Constants;
using AuthTest_2025_03.Identity.Data;
using AuthTest_2025_03.Models.Exceptions;
using AuthTest_2025_03.Models.Requests;
using AuthTest_2025_03.Services.Interfaces;
using AuthTest_2025_03.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace AuthTest_2025_03.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailService emailService, 
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
        }

        public async Task GenerateEmailConfirmation(ApplicationUser user, HttpRequest request)
        {
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId)); // Endode method used to encode url safe token
                var callbackURL = $"{request.Scheme}://{request.Host}/Authentication/ConfirmEmail?userId={userId}&token={token}";

                var emailMessage = $"Please confirm your account by <a href='{callbackURL}'>clicking here</a>.";

                await _emailService.SendEmailAsync(new() { ToEmails = [user.Email], Subject = "Confirm your email", HTMLContent = emailMessage });
            }

            throw new ArgumentException("Email address cannot be blank or null");
        }

        public async Task GenerateForgotPasswordEmail(string email, HttpRequest request)
        {
            // TODO : MRB Add support for username OR email later
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            try
            {
                // TODO : MRB Adjust so that this token is only good for a certain amount of time.
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string encodedToken = HttpUtility.UrlEncode(token); // used to encode url safe query string
                var encodedUsername = HttpUtility.UrlEncode(email);

                var callbackURL = $"{request.Scheme}://{request.Host}/Authentication/ResetPassword?user={encodedUsername}&token={encodedToken}";
                var emailMessage = $"Reset your password <a href=\"{callbackURL}\">here.</a>";

                await _emailService.SendEmailAsync(new() { ToEmails = [user.Email!], Subject = "Reset your password", HTMLContent = emailMessage });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task InvalidateAndIssueNewCookie(ClaimsPrincipal user)
        {
            var userResult = await _userManager.GetUserAsync(user);
            if (userResult == null)
            {
                throw new NotFoundException("User not found");
            }

            await _signInManager.SignOutAsync();

            var rememberMeClaim = user.FindFirst("RememberMe");
            var isRememberMe = rememberMeClaim != null && bool.Parse(rememberMeClaim.Value);

            await SignInWithUserClaims(isRememberMe, userResult);

        }

        public async Task<bool> IsEmailAlreadyInUse(string email)
        {
            return (await _userManager.FindByEmailAsync(email) != null);

        }

        public async Task<bool> IsUsernameAlreadyInUse(string username)
        {
            return (await _userManager.FindByNameAsync(username) != null);
        }

        public async Task Login(LoginRequest request)
        {           

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            //var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe, lockoutOnFailure: false);
            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordCheck)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            await SignInWithUserClaims(request.RememberMe, user);

            // TODO : MRB return expiration time to allow for refresh before session expiration

        }

        private async Task SignInWithUserClaims(bool rememberMe, ApplicationUser user)
        {
            // Get user claims
            var claims = await _userManager.GetClaimsAsync(user);

            // Add Id, UserName , and RememberMe to claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName!)); // TODO : MRB UserName won't (shouldn't) be null here
            claims.Add(new Claim("RememberMe", rememberMe.ToString()));

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            await _signInManager.SignInWithClaimsAsync(user, rememberMe, claims);
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task RegisterUserAsync(RegisterRequest model, HttpRequest request)
        {

            if (await IsEmailAlreadyInUse(model.Email))
            {
                // We should be checking for this on the frontend
                // but we should also check again on the backend
                throw new InvalidOperationException("Email already in use");
            }

            string userName = UsernameUtility.ExtractUsernameFromEmail(model.Email);

            if (await IsUsernameAlreadyInUse(userName))
            {
                // Way to set initial username if it is already taken
                // they can change it after they login
                userName = UsernameUtility.GenerateUniqueUsername(userName);
            }

            ApplicationUser? userExists = await _userManager.FindByNameAsync(userName);
            if (userExists != null)
            {
                throw new InvalidOperationException("User already exists");
            }

            // Create user object
            ApplicationUser user = new ApplicationUser
            {
                UserName = userName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true, // TODO : MRB Implement email confirmation
            };

            // TODO : MRB If an error happens after here, catch the error, remove this record , and rethrow the error
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));
            }

            // TODO : MRB Implement better way to create roles
            // Add roles
            await _userManager.AddToRoleAsync(user, ApplicationRole.SiteAdmin.ToString());
        }
    }
}
