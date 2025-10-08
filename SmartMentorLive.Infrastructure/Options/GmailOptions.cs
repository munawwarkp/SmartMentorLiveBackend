using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Infrastructure.Options
{
    public class GmailOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserEmail { get; set; }
        public string RedirectUri { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
