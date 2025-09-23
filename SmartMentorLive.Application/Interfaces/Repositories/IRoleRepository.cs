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
        Task<Role> AddAsync(Role role,CancellationToken cancellationToken);
        Task<int> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken);
    }
}
