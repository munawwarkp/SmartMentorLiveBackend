using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartMentorLive.Application.Features.Auth.Interfaces;
using SmartMentorLive.Infrastructure.Entities;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;
        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken ?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            string tokenHash = HashToken(token);

            var res = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash  == tokenHash,cancellationToken);
            return res;
        }

        public async Task RevokeAsync(string token, CancellationToken cancellationToken)
        {
            string tokenHash = HashToken(token);

            var rt = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash,cancellationToken);
            if (rt == null) return;

            rt.RevokedAtUtc = DateTime.UtcNow;
            _context.RefreshTokens.Update(rt);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private string HashToken(string token)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);   
        }
    }
}
