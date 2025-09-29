using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.Repositories
{
    public class LoginHistoryRepository : ILoginHistoryRepository
    {
        private readonly AppDbContext _context;
        public LoginHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LoginHistory loginHistory, CancellationToken cancellationToken)
        {
            await _context.LoginHistories.AddAsync(loginHistory, cancellationToken);
            //save changes is handled by unit of work
        }
    }
}
