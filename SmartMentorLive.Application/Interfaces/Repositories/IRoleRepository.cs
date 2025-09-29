using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role,CancellationToken cancellationToken);
        Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken);
    }
}
