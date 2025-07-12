using AlarmTracking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class RefreshToken : BaseAuditableEntity
    {
        public string Token { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public string? RevokedReason { get; private set; }

        // Navigation property
        public User User { get; private set; } = null!;

        // Private constructor for EF Core
        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, DateTime expiresAt)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = GenerateSecureToken(),
                UserId = userId,
                ExpiresAt = expiresAt,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Revoke(string reason = "Manually revoked")
        {
            IsRevoked = true;
            RevokedReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
