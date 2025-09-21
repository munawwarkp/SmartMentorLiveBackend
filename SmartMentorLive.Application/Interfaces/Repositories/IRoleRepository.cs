using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<int> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken);
    }
}
