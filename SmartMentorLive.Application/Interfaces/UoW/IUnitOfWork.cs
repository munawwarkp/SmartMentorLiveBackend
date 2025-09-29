using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Application.Interfaces.Repositories;

namespace SmartMentorLive.Infrastructure.Persistence.UOW
{
    public interface IUnitOfWork:IDisposable
    {
        Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
