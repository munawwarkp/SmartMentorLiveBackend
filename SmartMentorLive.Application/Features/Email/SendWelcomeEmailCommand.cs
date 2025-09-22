using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SmartMentorLive.Application.Features.Email
{
    public class SendWelcomeEmailCommand:IRequest
    {
        public string RecipientEmail {  get; set; }
        public string Name {  get; set; }
    }
}
