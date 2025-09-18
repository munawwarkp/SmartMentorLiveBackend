using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Industries;
using SmartMentorLive.Domain.Entities.Sessions;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Domain.Entities.Subscriptions
{
    public class SubscriptionPlan:BaseEntity
    {
        public string Name { get; set; } = default!;   // "Basic", "Pro", "Enterprise"
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }  // e.g. 30 days

        //depend on industry table,thats why fk of industryiid here
        public int IndustryId {  get; set; }
        public Industry Industry { get; set; }
        public ICollection<SubscriptionFeature> Features { get; set; }
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; }=new List<StudentSubscription>();
        public ICollection<MentorSubscriptionPlan> MentorSubscriptionPlans { get; set; } = new List<MentorSubscriptionPlan>();

        //distinct videos per plan
        public ICollection<PlanRecordedVideo> PlanRecordedVideos { get; set; } = new List<PlanRecordedVideo>();


    }
}
