using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.ObjectPool;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.Repositories
{
    public class RoleRepository:IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken)
        {
            await _context.Roles.AddAsync(role,cancellationToken);
            
        }

        public async Task<List<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
           return await _context.Roles.AsNoTracking().ToListAsync(cancellationToken);
        }


        public async Task<bool> ExistsByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _context.Roles
                .AsNoTracking()
                .AnyAsync(r => r.Name.ToLower() == roleName.ToLower(), cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(string roleName,CancellationToken cancellationToken)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower(),cancellationToken);
        }


        //public async Task GetAllowedRegistrationRolesAsync(CancellationToken cancellationToken)
        //{
        //   return await _context.Roles
        //        .Where(r => r.isre)

        //}
    }
}
