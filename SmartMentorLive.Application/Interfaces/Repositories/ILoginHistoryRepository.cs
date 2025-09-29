using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Interfaces.Repositories
{
    public interface ILoginHistoryRepository
    {
        Task AddAsync(LoginHistory loginHistory, CancellationToken cancellationToken = default);

        //Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId);
        //Task<LoginHistory?> GetLastLoginAsync(int userId);
    }
}
