using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;

namespace AlarmTracking.Application.Entities
{
    public class User : BaseAuditableEntity
    {
        public string Username { get; private set; } = string.Empty; 
        public string Email { get; private set; } = string.Empty; 
        public string PasswordHash { get; private set; } = string.Empty; 
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? Department { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        // Navigation properties (for JWT refresh tokens)
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        // Private constructor for EF Core
        private User() { }

        // Factory method for creating new users
        public static User Create(
            string username,
            string email,
            string passwordHash,
            string? firstName = null,
            string? lastName = null,
            string? department = null,
            UserRole role = UserRole.User) 
        {
            // Domain validation
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

            if (username.Length < 3 || username.Length > 50)
                throw new ArgumentException("Username must be between 3 and 50 characters", nameof(username));

            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format", nameof(email));

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                FirstName = firstName?.Trim(),
                LastName = lastName?.Trim(),
                Department = department?.Trim(),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return user;
        }

        public void UpdateProfile(string? firstName, string? lastName, string? department)
        {
            FirstName = firstName?.Trim();
            LastName = lastName?.Trim();
            Department = department?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRole(UserRole newRole)
        {
            Role = newRole;
            UpdatedAt = DateTime.UtcNow;
        }

        public string GetFullName()
        {
            if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
                return Username;

            return $"{FirstName} {LastName}".Trim();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
