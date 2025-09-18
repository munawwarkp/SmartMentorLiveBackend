using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Subscriptions;

namespace SmartMentorLive.Domain.Entities.Sessions
{
    public class PlanRecordedVideo:BaseEntity
    {
        public int SubscriptionPlanId {  get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }

        public int RecordedVideoId {  get; set; }
        public RecordedVideo RecordedVideo { get; set; }
    }
}
