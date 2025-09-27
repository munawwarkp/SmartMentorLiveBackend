using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Infrastructure.Persistence.Context;
using SmartMentorLive.Infrastructure.Persistence.UOW;

namespace SmartMentorLive.Infrastructure.Persistence.UoW
{
    public class AuthUnitOfWork:IAuthUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? transaction;    
        public AuthUnitOfWork(AppDbContext context, IUserRepository users, IRoleRepository roles)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(transaction != null)
                await transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if(transaction != null)
                await transaction.RollbackAsync(cancellationToken);
        }

        public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
           return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            transaction?.Dispose();
            _context.Dispose();
        }
    }
}
