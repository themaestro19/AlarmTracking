using AlarmTracking.Application.Contracts.Infrastructure;
using AlarmTracking.Application.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpiryMinutes;
        private readonly int _refreshTokenExpiryDays;
        private readonly ILogger _logger = Log.ForContext<JwtService>();

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer");
            _audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience");
            _accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "30");
            _refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");
        }

        public string GenerateAccessToken(User user)
        {
            try
            {
                _logger.Debug("Generating access token for user {UserId} ({Username})", user.Id, user.Username);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role.ToString()),
                    new("full_name", user.GetFullName()),
                    new("department", user.Department ?? ""),
                    new("is_active", user.IsActive.ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.Information("Access token generated successfully for user {UserId}, expires at {ExpiryTime}",
                    user.Id, tokenDescriptor.Expires);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error generating access token for user {UserId}", user.Id);
                throw;
            }
        }

        public RefreshToken GenerateRefreshToken(Guid userId)
        {
            try
            {
                _logger.Debug("Generating refresh token for user {UserId}", userId);

                var expiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                var refreshToken = RefreshToken.Create(userId, expiresAt);

                _logger.Information("Refresh token generated successfully for user {UserId}, expires at {ExpiryTime}",
                    userId, expiresAt);

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error generating refresh token for user {UserId}", userId);
                throw;
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                _logger.Debug("Token validated successfully");
                return principal;
            }
            catch (Exception ex)
            {
                _logger.Warning("Token validation failed: {Error}", ex.Message);
                return null;
            }
        }

        public Guid? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    _logger.Debug("Extracted user ID {UserId} from token", userId);
                    return userId;
                }

                _logger.Warning("Could not extract valid user ID from token");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error extracting user ID from token");
                return null;
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                var isExpired = jsonToken.ValidTo < DateTime.UtcNow;

                _logger.Debug("Token expiry check: {IsExpired}, ValidTo: {ValidTo}", isExpired, jsonToken.ValidTo);
                return isExpired;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error checking token expiry, treating as expired");
                return true;
            }
        }
    }
}
