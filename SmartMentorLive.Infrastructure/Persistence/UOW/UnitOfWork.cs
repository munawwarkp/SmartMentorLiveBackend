using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangeAsync(CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }

    }
}
