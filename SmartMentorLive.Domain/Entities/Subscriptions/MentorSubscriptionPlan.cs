using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Domain.Entities.Subscriptions
{
    public class MentorSubscriptionPlan:BaseEntity
    {
        public int MentorProfileId {  get; set; }
        public MentorProfile MentorProfile { get; set; }

        public int SubscriptionPlanId {  get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }
    }
}
