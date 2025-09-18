using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetEmailAsync(string email, CancellationToken cancellationToken=default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken=default);
        Task AddAsync(User user, CancellationToken cancellationToken=default);
    }
}
