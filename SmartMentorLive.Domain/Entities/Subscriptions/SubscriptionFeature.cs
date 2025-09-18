using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.Base;

namespace SmartMentorLive.Domain.Entities.Subscriptions
{
    public class SubscriptionFeature:BaseEntity
    {

        //fk links to SubscriptionPlan
        public int SubscriptionPlanId {  get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }

        //feature details
        public string Name {  get; set; }
        public string? Description {  get; set; }

        //Featur limits
        public int? Quantity { get; set; }
        public bool IsUnlimited { get; set; } = false;

    }
}
