// AlarmTracking.WebService/Controllers/AuthController.cs (Correct Serilog Usage)
using AlarmTracking.Application.Common.Exceptions;
using AlarmTracking.Application.Features.Users.RegisterUser;
using AlarmTracking.Application.Features.Users.Login;
using AlarmTracking.Application.Features.Users.RefreshToken;
using AlarmTracking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace AlarmTracking.WebService.Controllers.Authentication.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private static readonly Serilog.ILogger _logger = Log.ForContext<AuthController>();

        public AuthController(
            IUserService userService,
            IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>Created user information</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var contextLogger = _logger.ForContext("Operation", "UserRegistration")
                                      .ForContext("Email", request.Email);

            try
            {
                contextLogger.Information("Register endpoint called for email: {Email}", request.Email);

                var response = await _userService.RegisterUserAsync(request);

                contextLogger.Information("User registration successful for ID: {UserId}", response.Id);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = response.Id },
                    response);
            }
            catch (ValidationException ex)
            {
                contextLogger.Warning("Validation failed for user registration: {Email}. Errors: {@Errors}",
                    request.Email, ex.Errors);

                return BadRequest(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Extensions = { ["errors"] = ex.Errors }
                });
            }
            catch (ConflictException ex)
            {
                contextLogger.Warning("Conflict during user registration: {Email}. Message: {Message}",
                    request.Email, ex.Message);

                return Conflict(new ProblemDetails
                {
                    Title = "Conflict",
                    Detail = ex.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                contextLogger.Error(ex, "Unexpected error during user registration for email: {Email}", request.Email);

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Authenticate user and return JWT token
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT access token and user information</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var contextLogger = _logger.ForContext("Operation", "UserLogin")
                                      .ForContext("Email", request.Email);

            try
            {
                contextLogger.Information("Login endpoint called for email: {Email}", request.Email);

                var response = await _authService.LoginAsync(request);

                contextLogger.Information("Login successful for user: {UserId} ({Username})",
                    response.User.Id, response.User.Username);

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                contextLogger.Warning("Validation failed for login: {Email}", request.Email);

                return BadRequest(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (UnauthorizedException ex)
            {
                contextLogger.Warning("Unauthorized login attempt for email: {Email}", request.Email);

                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                contextLogger.Error(ex, "Unexpected error during login for email: {Email}", request.Email);

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Refresh JWT access token using refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>New JWT access token</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var contextLogger = _logger.ForContext("Operation", "TokenRefresh");

            try
            {
                contextLogger.Information("Token refresh endpoint called");

                var response = await _authService.RefreshTokenAsync(request);

                contextLogger.Information("Token refresh successful for user: {UserId} ({Username})",
                    response.User.Id, response.User.Username);

                return Ok(response);
            }
            catch (UnauthorizedException ex)
            {
                contextLogger.Warning("Unauthorized refresh token attempt");

                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                contextLogger.Error(ex, "Unexpected error during token refresh");

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Logout current user and revoke refresh tokens
        /// </summary>
        /// <returns>No content</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            var userId = GetCurrentUserId();
            var contextLogger = _logger.ForContext("Operation", "UserLogout")
                                      .ForContext("UserId", userId);

            try
            {
                contextLogger.Information("Logout endpoint called for user: {UserId}", userId);

                await _authService.LogoutAsync(userId);

                contextLogger.Information("Logout successful for user: {UserId}", userId);

                return NoContent();
            }
            catch (Exception ex)
            {
                contextLogger.Error(ex, "Unexpected error during logout for user: {UserId}", userId);

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Get current user information from JWT token
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var contextLogger = _logger.ForContext("Operation", "GetCurrentUser")
                                      .ForContext("UserId", userId);

            try
            {
                contextLogger.Information("GetCurrentUser endpoint called for user: {UserId}", userId);

                var response = await _userService.GetUserByIdAsync(userId);

                contextLogger.Debug("Current user retrieved successfully: {UserId} ({Username})",
                    response.Id, response.Username);

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                contextLogger.Warning("Current user not found: {UserId}", userId);

                return NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                contextLogger.Error(ex, "Unexpected error getting current user: {UserId}", userId);

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User information</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var contextLogger = _logger.ForContext("Operation", "GetUserById")
                                      .ForContext("TargetUserId", id)
                                      .ForContext("CurrentUserId", currentUserId);

            try
            {
                contextLogger.Information("GetUserById endpoint called for ID: {UserId} by user: {CurrentUserId}",
                    id, currentUserId);

                var response = await _userService.GetUserByIdAsync(id);

                contextLogger.Information("User retrieved successfully for ID: {UserId}", id);

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                contextLogger.Warning("User not found: {UserId}. Message: {Message}", id, ex.Message);

                return NotFound(new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                contextLogger.Error(ex, "Unexpected error retrieving user with ID: {UserId}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing your request.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            _logger.Error("Failed to extract valid user ID from JWT token. UserIdClaim: {UserIdClaim}", userIdClaim);
            throw new UnauthorizedException("Invalid user context");
        }
    }
}