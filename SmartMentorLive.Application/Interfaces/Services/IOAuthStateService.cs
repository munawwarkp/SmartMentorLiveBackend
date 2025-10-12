using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Application.Interfaces.Services
{
    public interface IOAuthStateService
    {
        Task StoreStateAsync(string state, TimeSpan? expiration);
        Task<bool> ValidateStateAsync(string state);
    }
}
