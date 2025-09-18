using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();

        //implementation goest into infrastructure layer bcs,
        //* token generation requires external libraries(jwt,..)
        //* depends on configuration
        
    }
}
    