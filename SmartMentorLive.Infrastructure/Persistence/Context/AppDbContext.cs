using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartMentorLive.Domain.Entities.admin;
using SmartMentorLive.Domain.Entities.Industries;
using SmartMentorLive.Domain.Entities.Notifications;
using SmartMentorLive.Domain.Entities.Oauth;
using SmartMentorLive.Domain.Entities.Sessions;
using SmartMentorLive.Domain.Entities.Subscriptions;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Entities;

namespace SmartMentorLive.Infrastructure.Persistence.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<MentorProfile> MentorProfiles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<MentorEarning> MentorEarnings { get; set; }
        public DbSet<MentorAvalability> MentorAvalabilities { get; set; }

        
        public DbSet<Industry> Industries { get; set; }
        public DbSet<MentorIndustry> MentorIndustries  { get; set; }

        
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<MentorSubscriptionPlan> MentorSubscriptionPlans { get; set; }
        public DbSet<PaymentTransaction> PaymentTransaction { get; set; }


        public DbSet<GroupSession> GroupSessions { get; set; }
        public DbSet<GroupSessionEnrollment> GetGroupSessionEnrollments { get; set; }
        public DbSet<MentorshipSessionBooking> MentorshipSessionBookings { get; set; }
        public DbSet<PlanRecordedVideo> PlanRecordedVideo { get; set; }
        public DbSet<RecordedVideo> RecordedVideos { get; set; }
        public DbSet<SessionHistory> SessionHistory { get; set; }
        public DbSet<SessionRating> SessionRating { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<AdminAuditLog> AdminAuditLog { get; set; }
        public DbSet<EarningPayout> EarningPayouts {  get; set; }


        public DbSet<RefreshToken> RefreshTokens { get; set; }

        //
        public DbSet<OAuthToken> OAuthTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //user
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.StudentProfile)
                .WithOne(s => s.User)
                .HasForeignKey<StudentProfile>(s => s.UserId)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasOne(u => u.MentorProfile)
                .WithOne(m => m.User)
                .HasForeignKey<MentorProfile>(m => m.UserId)
                .IsRequired(false);
            //industry

            //subscription
            modelBuilder.Entity<SubscriptionPlan>()
                .HasMany(s => s.Features)
                .WithOne(f => f.SubscriptionPlan)
                .OnDelete(DeleteBehavior.Cascade);

            //StudentSubscription -> StudentProfile
            modelBuilder.Entity<StudentSubscription>()
                .HasOne(sb => sb.StudentProfile)
                .WithMany(sp => sp.StudentSubscriptions)
                .HasForeignKey(sb => sb.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            //StudentSubscription -> PaymentTransaction
            modelBuilder.Entity<StudentSubscription>()
                .HasMany(sb => sb.PaymentTransactions)
                .WithOne(pt => pt.StudentSubscription)
                .HasForeignKey(sb => sb.StudentSubscriptionId)
                .OnDelete(DeleteBehavior.Restrict); 

            //studentSubscription -> subscriptionPlan
            modelBuilder.Entity<StudentSubscription>()
                .HasOne(ss => ss.SubscriptionPlan)
                .WithMany(sp => sp.StudentSubscriptions)
                .HasForeignKey(ss => ss.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            //PaymentTransation -> StudentProfile
            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.StudentProfile)
                .WithMany(sp => sp.PaymentTransactions)
                .HasForeignKey(pt => pt.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

        
            //mentorSubscription
            modelBuilder.Entity<MentorSubscriptionPlan>()
                .HasOne(ms => ms.MentorProfile)
                .WithMany(mp => mp.MentorSubscriptionPlans)
                .HasForeignKey(ms => ms.MentorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            //videolevel enum stored as string instead of int
            modelBuilder.Entity<RecordedVideo>()
                .Property(v => v.VideolLevel)
                .HasConversion<string>();

            // prevent multiple rating from same student
            modelBuilder.Entity<SessionRating>()
                .HasIndex(r => new { r.StudentProfileId, r.MentorshipSessionBookingId })
                .IsUnique()
                .HasFilter("[MentorshipSessionBookingId] IS NOT NULL");
            modelBuilder.Entity<SessionRating>()
                .HasIndex(r => new { r.StudentProfileId, r.GroupSessionId })
                .IsUnique()
                .HasFilter("[GroupSessionId]  IS NOT NULL");

            modelBuilder.Entity<PaymentTransaction>(e =>
                e.Property(p => p.Amount)
                .HasPrecision(18, 2)
            );

            modelBuilder.Entity<SubscriptionPlan>(e =>
                e.Property(p => p.Price)
                .HasPrecision(18, 2)
            );

            modelBuilder.Entity<EarningPayout>(ent =>
            {
                ent.Property(p => p.TotalGross).HasPrecision(18, 2);
                ent.Property(p => p.TotalCommission).HasPrecision(18, 2);
                ent.Property(p => p.NetAmount).HasPrecision(18, 2);
            }
            );

            //industry-mentor

            modelBuilder.Entity<MentorIndustry>()
                .HasKey(mi => new { mi.MentorProfileId, mi.IndustryId }); //composite primary key

            modelBuilder.Entity<MentorIndustry>()
                .HasOne(mi => mi.MentorProfile)
                .WithMany(mi => mi.MentorIndustries)
                .HasForeignKey(mi => mi.MentorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MentorIndustry>()
                .HasOne(mi => mi.Industry)
                .WithMany(i => i.MentorIndustries)
                .HasForeignKey(mi => mi.IndustryId)
                .OnDelete(DeleteBehavior.Restrict);

            //

            modelBuilder.Entity<GroupSessionEnrollment>()
                  .HasOne(e => e.StudentProfile)
                  .WithMany()
                  .HasForeignKey(e => e.StudentProfileId)
                  .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction

  
           modelBuilder.Entity<PlanRecordedVideo>()
                .HasKey(prv => new {prv.SubscriptionPlanId,prv.RecordedVideoId});

            modelBuilder.Entity<PlanRecordedVideo>()
                .HasOne(prv => prv.SubscriptionPlan)
                .WithMany(sp => sp.PlanRecordedVideos)
                .HasForeignKey(prv => prv.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlanRecordedVideo>()
                .HasOne(prv => prv.RecordedVideo)
                .WithMany(sp => sp.PlanRecordedVideos)
                .HasForeignKey(prv => prv.RecordedVideoId)
                .OnDelete(DeleteBehavior.Restrict);

            //MentorEarning
            modelBuilder.Entity<MentorEarning>(ent =>
            {
                ent.Property(p => p.GrossAmount).HasPrecision(18, 2);
                ent.Property(p => p.Commission).HasPrecision(18, 2);
                ent.Property(p => p.NetAmount).HasPrecision(18, 2);
            });



            //refresh token
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(u => u.UserId).IsRequired();
            });

            //Oauth
            modelBuilder.Entity<OAuthToken>()
                .HasIndex(t => t.UserEmail)
                .IsUnique();


        }
    }
}
