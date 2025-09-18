using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartMentorLive.Domain.Entities.admin;
using SmartMentorLive.Domain.Entities.Base;
using SmartMentorLive.Domain.Entities.Industries;
using SmartMentorLive.Domain.Entities.Subscriptions;
using SmartMentorLive.Domain.Enums;

namespace SmartMentorLive.Domain.Entities.Users
{
    public class MentorProfile:BaseEntity
    {
        //  Link to User
        public int UserId { get; set; }
        public User User { get; set; }

        //  Contact
        public string? Phone { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Mentor-specific info
        public string? Bio { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? ResumeUrl { get; set; }  //resume


        //Approval
        public MentorApprovalStatus ApprovalStatus { get; private set; } = MentorApprovalStatus.Pending;
        public string? RejectionReason { get; private set; }


        //skills and industries
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<MentorIndustry> MentorIndustries { get; set; } = new List<MentorIndustry>();



        public ICollection<EarningPayout> EarningPayouts { get; set; } = new List<EarningPayout>();
        public ICollection<MentorSubscriptionPlan> MentorSubscriptionPlans { get; set; } = new List<MentorSubscriptionPlan>();
      


        //domain method
        public void Approve()
        {
            ApprovalStatus = MentorApprovalStatus.Approved;
            RejectionReason = null;
        }

        public void Reject(string reason)
        {
            ApprovalStatus = MentorApprovalStatus.Rejected;
            RejectionReason = reason;
        }

        public void CanCreateSession()
        {
            if(ApprovalStatus == MentorApprovalStatus.Pending)
                ApprovalStatus = MentorApprovalStatus.Approved;

        }
      
    }
}
