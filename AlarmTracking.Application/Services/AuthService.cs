using AlarmTracking.Application.Common.Exceptions;
using AlarmTracking.Application.Contracts.Infrastructure;
using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Features.Users.Login;
using AlarmTracking.Application.Features.Users.RefreshToken;
using Serilog;

namespace AlarmTracking.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly ILogger _logger = Log.ForContext<AuthService>();

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var contextLogger = _logger.ForContext("Operation", "UserLogin")
                          .ForContext("Email", request.Email);

            try
            {
                _logger.Information("Login attempt started for email: {Email}", request.Email);

                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.Warning("Login failed: Missing email or password for {Email}", request.Email);
                    throw new ValidationException("Email and password are required.");
                }

                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    _logger.Warning("Login failed: User not found for email: {Email}", request.Email);
                    throw new UnauthorizedException("Invalid email or password.");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.Warning("Login failed: User account deactivated for {Email} (UserId: {UserId})",
                        request.Email, user.Id);
                    throw new UnauthorizedException("Account is deactivated.");
                }

                // Verify password
                if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                {
                    _logger.Warning("Login failed: Invalid password for {Email} (UserId: {UserId})",
                        request.Email, user.Id);
                    throw new UnauthorizedException("Invalid email or password.");
                }

                // Update last login
                user.UpdateLastLogin();
                await _userRepository.UpdateAsync(user);

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

                // Save refresh token
                await _refreshTokenRepository.AddAsync(refreshToken);

                _logger.Information("Login successful for user {UserId} ({Username}) from email {Email}",
                    user.Id, user.Username, user.Email);

                return new LoginResponse(
                    accessToken,
                    "Bearer",
                    1800, // 30 minutes
                    new UserInfo(
                        user.Id,
                        user.Username,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.Department,
                        new[] { user.Role.ToString() }));
            }
            catch (Exception ex) when (!(ex is UnauthorizedException || ex is ValidationException))
            {
                _logger.Error(ex, "Unexpected error during login for email: {Email}", request.Email);
                throw;
            }
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var contextLogger = _logger.ForContext("Operation", "TokenRefresh");

            try
            {
                _logger.Information("Token refresh attempt started");

                var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
                if (refreshToken == null || !refreshToken.IsValid)
                {
                    _logger.Warning("Token refresh failed: Invalid or expired refresh token");
                    throw new UnauthorizedException("Invalid or expired refresh token.");
                }

                var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
                if (user == null || !user.IsActive)
                {
                    _logger.Warning("Token refresh failed: User not found or inactive for UserId: {UserId}",
                        refreshToken.UserId);
                    throw new UnauthorizedException("User not found or inactive.");
                }

                // Revoke old refresh token
                refreshToken.Revoke("Used for refresh");
                await _refreshTokenRepository.UpdateAsync(refreshToken);

                // Generate new tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);

                // Save new refresh token
                await _refreshTokenRepository.AddAsync(newRefreshToken);

                _logger.Information("Token refresh successful for user {UserId} ({Username})",
                    user.Id, user.Username);

                return new LoginResponse(
                    accessToken,
                    "Bearer",
                    1800, // 30 minutes
                    new UserInfo(
                        user.Id,
                        user.Username,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.Department,
                        new[] { user.Role.ToString() }));
            }
            catch (Exception ex) when (!(ex is UnauthorizedException))
            {
                _logger.Error(ex, "Unexpected error during token refresh");
                throw;
            }
        }

        public async Task LogoutAsync(Guid userId)
        {
            try
            {
                _logger.Information("Logout initiated for user: {UserId}", userId);

                // Revoke all refresh tokens for the user
                await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);

                _logger.Information("Logout completed successfully for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during logout for user: {UserId}", userId);
                throw;
            }
        }

        public async Task RevokeAllTokensAsync(Guid userId)
        {
            try
            {
                _logger.Information("Revoking all tokens for user: {UserId}", userId);

                await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);

                _logger.Information("All tokens revoked successfully for user: {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error revoking tokens for user: {UserId}", userId);
                throw;
            }
        }
    }
}
