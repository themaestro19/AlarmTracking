using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.DataAccess.Repositories.Dapper;
using AlarmTracking.DataAccess.Repositories.EntityFramework;
using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.Repositories
{
    public class HybridRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly EfRefreshTokenRepository _efRepository;
        private readonly DapperRefreshTokenRepository _dapperRepository;

        public HybridRefreshTokenRepository(
            EfRefreshTokenRepository efRepository,
            DapperRefreshTokenRepository dapperRepository)
        {
            _efRepository = efRepository;
            _dapperRepository = dapperRepository;
        }

        // Use EF Core for write operations
        public async Task<RefreshToken> AddAsync(RefreshToken refreshToken) =>
            await _efRepository.AddAsync(refreshToken);

        public async Task UpdateAsync(RefreshToken refreshToken) =>
            await _efRepository.UpdateAsync(refreshToken);

        public async Task DeleteAsync(Guid id) =>
            await _efRepository.DeleteAsync(id);

        public async Task RevokeAllUserTokensAsync(Guid userId) =>
            await _efRepository.RevokeAllUserTokensAsync(userId);

        // Use Dapper for read operations (better performance)
        public async Task<RefreshToken?> GetByTokenAsync(string token) =>
            await _dapperRepository.GetByTokenAsync(token);

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId) =>
            await _dapperRepository.GetByUserIdAsync(userId);
    }
}
