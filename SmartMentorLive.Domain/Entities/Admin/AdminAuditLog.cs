using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.admin
{
    //record admin action  like blocking a use,approve mentor, update subscription
    public class AdminAuditLog : BaseEntity
    {
        //who performed the action
        public int AdminId { get; set; }


        //action type : block user, approve mentor,update subscription,...
        public string ActionType { get; set; }

        //on which entity this action is occured
        public string EntityType { get; set; }
        public int EntityId { get; set; }


        public string? Description {  get; set; }

        public bool IsSuccess { get; set; } = false;
    }
}
