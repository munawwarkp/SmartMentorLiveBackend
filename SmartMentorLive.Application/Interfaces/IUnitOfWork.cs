using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangeAsync(CancellationToken cancellationToken);
    }
}
