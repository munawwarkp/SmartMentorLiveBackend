using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Application.Interfaces.Services
{
    public interface ITokenManager
    {
        Task<string> GetAccessTokenAsync();

    }
}
