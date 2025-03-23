using AuthTest_2025_03.Identity.Data;
using AuthTest_2025_03.Models.Requests;
using System.Security.Claims;

namespace AuthTest_2025_03.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> IsEmailAlreadyInUse(string email);
        Task RegisterUserAsync(RegisterRequest model, HttpRequest request);
        Task<bool> IsUsernameAlreadyInUse(string username);
        Task GenerateEmailConfirmation(ApplicationUser user, HttpRequest request);
        Task GenerateForgotPasswordEmail(string email, HttpRequest request);
        Task Login(LoginRequest request);
        Task Logout();
        Task InvalidateAndIssueNewCookie(ClaimsPrincipal user);
    }
}
