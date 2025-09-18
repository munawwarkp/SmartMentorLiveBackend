using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Subscriptions;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Domain.Entities.Industries
{
    public class Industry:BaseEntity
    {
        public string Name { get; set; }
        public string? Description {  get; set; }

        public ICollection<SubscriptionPlan>? SubscriptionPlans { get; set; }

        public ICollection<MentorIndustry> MentorIndustries { get; set; } = new List<MentorIndustry>();
        //public ICollection<MentorProfile> Mentors {  get; set; }=new List<MentorProfile>();
    }
}
