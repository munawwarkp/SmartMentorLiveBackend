using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMentorLive.Application.Features.Auth.Dtos
{
    public class RegisterResultDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Student"; // default role




        // Optional: if you want to support verification later
        //public bool IsEmailConfirmed { get; set; } = false;
    }
}
