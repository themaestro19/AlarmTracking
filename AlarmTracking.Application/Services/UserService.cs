using AlarmTracking.Application.Common.Exceptions;
using AlarmTracking.Application.Contracts.Infrastructure;
using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Features.Users.RegisterUser;
using AlarmTracking.Application.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ValidationException = AlarmTracking.Application.Common.Exceptions.ValidationException;

namespace AlarmTracking.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger = Log.ForContext<UserService>();

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request)
        {
            try
            {
                _logger.Information("Starting user registration for email: {Email}", request.Email);

                // Validate the request
                await ValidateRegistrationRequest(request);

                // Check if user already exists by email
                var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    _logger.Warning("Registration failed: User with email {Email} already exists", request.Email);
                    throw new ConflictException($"A user with email '{request.Email}' already exists.");
                }

                // Check if username is taken
                var existingUserByUsername = await _userRepository.GetByUsernameAsync(request.Username);
                if (existingUserByUsername != null)
                {
                    _logger.Warning("Registration failed: Username {Username} is already taken", request.Username);
                    throw new ConflictException($"Username '{request.Username}' is already taken.");
                }

                // Hash the password
                var hashedPassword = _passwordHasher.Hash(request.Password);

                // Create domain entity
                var user = User.Create(
                    request.Username,
                    request.Email,
                    hashedPassword,
                    request.FirstName,
                    request.LastName,
                    request.Department);

                // Save user to database
                var savedUser = await _userRepository.AddAsync(user);

                _logger.Information("User registered successfully with {UserId} and username {Username}",
                    savedUser.Id, savedUser.Username);

                // Return response
                return new RegisterUserResponse(
                    savedUser.Id,
                    savedUser.Username,
                    savedUser.Email,
                    savedUser.FirstName,
                    savedUser.LastName,
                    savedUser.Department,
                    savedUser.CreatedAt);
            }
            catch (Exception ex) when (!(ex is ConflictException || ex is ValidationException))
            {
                _logger.Error(ex, "Error occurred during user registration for email: {Email}", request.Email);
                throw;
            }
        }

        public async Task<GetUserResponse> GetUserByIdAsync(Guid id)
        {
            _logger.Information("Retrieving user with ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.Warning("User with ID {UserId} not found", id);
                throw new NotFoundException($"User with ID '{id}' was not found.");
            }

            _logger.Debug("Successfully retrieved user {UserId} with username {Username}", user.Id, user.Username);

            return new GetUserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Department,
                user.CreatedAt);
        }

        public async Task<IEnumerable<GetUserResponse>> GetAllUsersAsync()
        {
            _logger.Information("Retrieving all users");

            var users = await _userRepository.GetAllAsync();
            var userList = users.ToList();

            _logger.Information("Retrieved {UserCount} users", userList.Count);

            return userList.Select(user => new GetUserResponse(
                user.Id,
                user.Username,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Department,
                user.CreatedAt));
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserRequest request)
        {
            _logger.Information("Updating user with ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.Warning("Update failed: User with ID {UserId} not found", id);
                throw new NotFoundException($"User with ID '{id}' was not found.");
            }

            var oldValues = new { user.FirstName, user.LastName, user.Department };
            user.UpdateProfile(request.FirstName, request.LastName, request.Department);
            await _userRepository.UpdateAsync(user);

            _logger.Information("User {UserId} updated successfully. Old values: {@OldValues}, New values: {@NewValues}",
                id, oldValues, new { request.FirstName, request.LastName, request.Department });
        }

        public async Task DeleteUserAsync(Guid id)
        {
            _logger.Information("Deleting user with ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.Warning("Delete failed: User with ID {UserId} not found", id);
                throw new NotFoundException($"User with ID '{id}' was not found.");
            }

            var deletedUserInfo = new { user.Username, user.Email };
            await _userRepository.DeleteAsync(id);

            _logger.Information("User {UserId} deleted successfully. Deleted user info: {@DeletedUserInfo}",
                id, deletedUserInfo);
        }

        private async Task ValidateRegistrationRequest(RegisterUserRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            // Username validation
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                errors.Add(nameof(request.Username), new[] { "Username is required." });
            }
            else if (request.Username.Length < 3 || request.Username.Length > 50)
            {
                errors.Add(nameof(request.Username), new[] { "Username must be between 3 and 50 characters." });
            }
            else if (!Regex.IsMatch(request.Username, @"^[a-zA-Z0-9_.-]+$"))
            {
                errors.Add(nameof(request.Username), new[] { "Username can only contain letters, numbers, dots, hyphens, and underscores." });
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                errors.Add(nameof(request.Email), new[] { "Email is required." });
            }
            else if (!new EmailAddressAttribute().IsValid(request.Email))
            {
                errors.Add(nameof(request.Email), new[] { "Email must be a valid email address." });
            }
            else if (request.Email.Length > 254)
            {
                errors.Add(nameof(request.Email), new[] { "Email must not exceed 254 characters." });
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                errors.Add(nameof(request.Password), new[] { "Password is required." });
            }
            else if (request.Password.Length < 8)
            {
                errors.Add(nameof(request.Password), new[] { "Password must be at least 8 characters long." });
            }
            else if (!Regex.IsMatch(request.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]"))
            {
                errors.Add(nameof(request.Password), new[] { "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character." });
            }

            // Optional field validations
            if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName.Length > 50)
            {
                errors.Add(nameof(request.FirstName), new[] { "First name must not exceed 50 characters." });
            }

            if (!string.IsNullOrEmpty(request.LastName) && request.LastName.Length > 50)
            {
                errors.Add(nameof(request.LastName), new[] { "Last name must not exceed 50 characters." });
            }

            if (!string.IsNullOrEmpty(request.Department) && request.Department.Length > 100)
            {
                errors.Add(nameof(request.Department), new[] { "Department must not exceed 100 characters." });
            }

            if (errors.Any())
            {
                _logger.Warning("Validation failed for user registration: {Email}. Errors: {@ValidationErrors}",
                    request.Email, errors);
                throw new ValidationException(errors);
            }
        }
    }
}
