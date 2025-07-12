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
    public class HybridUserRepository : IUserRepository
    {
        private readonly EfUserRepository _efRepository;
        private readonly DapperUserRepository _dapperRepository;

        public HybridUserRepository(EfUserRepository efRepository, DapperUserRepository dapperRepository)
        {
            _efRepository = efRepository;
            _dapperRepository = dapperRepository;
        }

        // Use EF Core for write operations
        public async Task<User> AddAsync(User user) => await _efRepository.AddAsync(user);
        public async Task UpdateAsync(User user) => await _efRepository.UpdateAsync(user);
        public async Task DeleteAsync(Guid id) => await _efRepository.DeleteAsync(id);

        // Use Dapper for read operations (better performance)
        public async Task<User?> GetByIdAsync(Guid id) => await _dapperRepository.GetByIdAsync(id);
        public async Task<User?> GetByEmailAsync(string email) => await _dapperRepository.GetByEmailAsync(email);
        public async Task<User?> GetByUsernameAsync(string username) => await _dapperRepository.GetByUsernameAsync(username);
        public async Task<IEnumerable<User>> GetAllAsync() => await _dapperRepository.GetAllAsync();
        public async Task<bool> ExistsAsync(Guid id) => await _dapperRepository.ExistsAsync(id);
    }
}
