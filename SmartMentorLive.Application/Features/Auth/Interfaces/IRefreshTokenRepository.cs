using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Infrastructure.Entities;

namespace SmartMentorLive.Application.Features.Auth.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task RevokeAsync(string token, CancellationToken cancellationToken);
    }
}
