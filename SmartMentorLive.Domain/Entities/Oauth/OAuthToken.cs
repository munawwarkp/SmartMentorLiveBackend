using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.Oauth
{
    public class OAuthToken:BaseEntity
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string TokenJson { get; set; }

    }
}
