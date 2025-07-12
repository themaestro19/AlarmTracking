using AlarmTracking.Application.Features.Users.RegisterUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface IUserService
    {
        Task<RegisterUserResponse> RegisterUserAsync(RegisterUserRequest request);
        Task<GetUserResponse> GetUserByIdAsync(Guid id);
        Task<IEnumerable<GetUserResponse>> GetAllUsersAsync();
        Task UpdateUserAsync(Guid id, UpdateUserRequest request);
        Task DeleteUserAsync(Guid id);
    }

    public record GetUserResponse(
        Guid Id,
        string Username,
        string Email,
        string? FirstName,
        string? LastName,
        string? Department,
        DateTime CreatedAt);

    public record UpdateUserRequest(
        string? FirstName,
        string? LastName,
        string? Department);
}
