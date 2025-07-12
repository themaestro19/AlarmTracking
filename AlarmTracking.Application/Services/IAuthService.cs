using AlarmTracking.Application.Features.Users.Login;
using AlarmTracking.Application.Features.Users.RefreshToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(Guid userId);
        Task RevokeAllTokensAsync(Guid userId);
    }
}
