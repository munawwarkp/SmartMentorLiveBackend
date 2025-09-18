using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Infrastructure.Entities
{
    public class RefreshToken:BaseEntity
    {
        public string TokenHash {  get; set; } // store hashed refreshtoken
        public string UserId {  get; set; } //userid always points to a real user
        public DateTime ExpiresAtUtc { get; set; }
        //public DateTime CreatedAtUtc { get; set; }
        //public string? CreatedByIp { get; set; }  //records Ip address from which the refresh token was issued
        public DateTime? RevokedAtUtc { get; set; } //eg:for logout time
        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsActive => RevokedAtUtc == null && !IsExpired;

    }
}
