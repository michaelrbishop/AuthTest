using AuthTest_2025_03.Identity.Data;
using AuthTest_2025_03.Models.Requests;
using AuthTest_2025_03.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest_2025_03.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        // TODO : MRB Get User Claims ??

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // TODO : MRB Login
            // On success return expiration time to allow for refresh before session expiration
            await _accountService.Login(request);
            return Ok();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // TODO : MRB Register
            await _accountService.RegisterUserAsync(request, HttpContext.Request);
            return Ok();
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Logout()
        {
            // TODO : MRB Logout
            await _accountService.Logout();
            return Ok();
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ProblemDetails))]
        public async Task<IActionResult> Refresh()
        {
            // TODO : MRB Refresh
            await _accountService.InvalidateAndIssueNewCookie(User);
            return Ok();
        }
    }
}
