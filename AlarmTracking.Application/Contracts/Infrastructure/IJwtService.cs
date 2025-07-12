using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Contracts.Infrastructure
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(Guid userId);
        ClaimsPrincipal? ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
        bool IsTokenExpired(string token);
    }
}
