using AlarmTracking.WebService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AlarmTracking.WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(
            IAuthenticationService authService,
            ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _authService.LoginAsync(command);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Error });

            var response = MapToAuthResponse(result.Value);
            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterCommand
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Department = request.Department
            };

            var result = await _authService.RegisterAsync(command);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            var response = MapToAuthResponse(result.Value);
            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand
            {
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken
            };

            var result = await _authService.RefreshTokenAsync(command);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Error });

            var response = MapToAuthResponse(result.Value);
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var command = new LogoutCommand
            {
                UserId = _currentUserService.UserId,
                RefreshToken = request.RefreshToken
            };

            var result = await _authService.LogoutAsync(command);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var command = new ChangePasswordCommand
            {
                UserId = _currentUserService.UserId,
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };

            var result = await _authService.ChangePasswordAsync(command);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            // This would typically fetch fresh user data from the database
            return Ok(new
            {
                id = _currentUserService.UserId,
                email = _currentUserService.Email,
                username = _currentUserService.Username,
                roles = _currentUserService.Roles,
                permissions = _currentUserService.Permissions
            });
        }

        private AuthResponse MapToAuthResponse(AuthenticationResult result)
        {
            return new AuthResponse
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                ExpiresAt = result.ExpiresAt,
                TokenType = "Bearer",
                User = new UserResponse
                {
                    Id = result.User.Id,
                    Email = result.User.Email,
                    Username = result.User.Username,
                    FirstName = result.User.FirstName,
                    LastName = result.User.LastName,
                    Department = result.User.Department,
                    Roles = result.User.Roles,
                    Permissions = result.User.Permissions
                }
            };
        }
    }
}
