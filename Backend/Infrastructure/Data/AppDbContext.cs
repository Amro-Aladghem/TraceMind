using Domain.Enities;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model
                                           .GetEntityTypes()
                                           .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<ChatRole>().HasData(
                new ChatRole { Id = 1, Name = "LoggingAgent" },
                new ChatRole { Id = 2, Name = "User" },
                new ChatRole { Id = 3, Name = "Tool" }
            );

            modelBuilder.Entity<Notification>().Property(p => p.NotficationStatus)
                .HasConversion(
                    status => status.ToString(),
                    status => Enum.Parse<eNotificationStatus>(status)
                );

            modelBuilder.Entity<Notification>().Property(p => p.AgentType)
                .HasConversion(
                    agentType => agentType.ToString(),
                    agentType => Enum.Parse<eAgentType>(agentType)
                );

            modelBuilder.Entity<Log>().Property(l => l.Level)
                .HasConversion(
                    l => l.ToString(),
                    l => Enum.Parse<eLogLevel>(l)
                );

            modelBuilder.Entity<AIInvestigation>().Property(A=> A.Status)
                .HasConversion(
                    A => A.ToString(),
                    A=> Enum.Parse<eInvestigationStatus>(A)
                );

            modelBuilder.Entity<Chat>().Property(p => p.AgentType)
                .HasConversion<string>();
        }

        
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<ChatRole> ChatRoles => Set<ChatRole>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AIInvestigation> AIInvestigations => Set<AIInvestigation>();
        public DbSet<Application> Applications => Set<Application>();
        public DbSet<Log> Logs => Set<Log>();
    }
}
