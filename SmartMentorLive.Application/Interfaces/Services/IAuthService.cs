using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Application.Features.Auth.Dtos;

namespace SmartMentorLive.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<RegisterResultDto> RegisterAsync(string name, string email, string password, string role, CancellationToken cancellationToken);
        Task<LoginResultDto> LoginAsync(string emain,string password, CancellationToken cancellationToken);
    }
}
