using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; } //fk to user
        public User User {  get; set; }
        public DateTime LoggedInAt { get; set; }

        //public string? IpAddress { get; set; }     // Optional: track IP
        //public string? Device { get; set; }        // Optional: track device/browser
    }
}
