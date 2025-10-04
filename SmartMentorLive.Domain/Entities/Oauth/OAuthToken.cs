using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.Oauth
{
    public class OAuthToken : BaseEntity
    {
        public int Id { get; set; }
        //Future-proof → You can add Outlook, LinkedIn, Zoom later by just changing Provider.
        public string Provider { get; set; } = "Gmail";
        public string UserEmail { get; set; }


        public string AccessTokenEncrypted { get; set; } = string.Empty;
        public string RefreshTokenEncrypted { get; set; } = string.Empty;


        public DateTime ExpiryDate { get; set; }

    }
}
