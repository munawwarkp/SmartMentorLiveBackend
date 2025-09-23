using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.Repositories
{
    public class RoleRepository:IRoleRepository
    {
        private readonly AppDbContext _context;
        private Dictionary<string, int> _roleCache;
        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role> AddAsync(Role role, CancellationToken cancellationToken)
        {
          
             _context.Roles.Add(role);
            await _context.SaveChangesAsync(cancellationToken);
            return role;
            
        }

        public async Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
           return await _context.Roles.ToListAsync(cancellationToken);
        }

        public async Task<int> GetRoleIdByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            if(_roleCache == null)
            {
                var roles = await _context.Roles
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                _roleCache = roles.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);
            }

            if(!_roleCache.TryGetValue(roleName,out var roleId))
                throw new Exception($"Role '{roleName}' not found ");

            return roleId;

        }

    }
}
