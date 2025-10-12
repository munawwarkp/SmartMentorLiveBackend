using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Infrastructure.Options
{
    public class RedisSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //Ssl false, bcs redis free cloud tire disabled tls(ssl) encryption
        public bool Ssl { get; set; }  
        public string InstanceName { get; set; } = string.Empty;

    }
}
