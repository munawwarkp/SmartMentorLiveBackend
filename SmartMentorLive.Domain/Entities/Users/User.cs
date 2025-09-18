using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class User:BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        //foreign key
        public int RoleId {  get; set; } //dependant table of role
        public Role Role { get; set; }


        // Navigation to profiles
        public StudentProfile? StudentProfile { get; set; }
        public MentorProfile? MentorProfile { get; set; }

    }
}
