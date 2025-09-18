using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Subscriptions;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Sessions
{
    public class RecordedVideo:BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url {  get; set; }
        public string? Thumbnail {  get; set; }


        //belongs to a single plan
        //public int SubscriptionPlanId { get; set; }

        public ICollection<PlanRecordedVideo> PlanRecordedVideos { get; set; }=new List<PlanRecordedVideo>();
        public VideolLevel VideolLevel { get; set; } //beginner/intermediate/adv
    }
}
