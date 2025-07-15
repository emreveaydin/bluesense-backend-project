using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Postgres.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupInvitation> GroupInvitations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // GroupMember join table configuration
        modelBuilder.Entity<GroupMember>()
            .HasKey(gm => new { gm.GroupId, gm.UserId });

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.GroupMembers)
            .HasForeignKey(gm => gm.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(gm => gm.UserId);
            
        // GroupInvitation configuration
        modelBuilder.Entity<GroupInvitation>()
            .HasOne(i => i.Group)
            .WithMany()
            .HasForeignKey(i => i.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GroupInvitation>()
            .HasOne(i => i.Inviter)
            .WithMany()
            .HasForeignKey(i => i.InviterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GroupInvitation>()
            .HasOne(i => i.Invitee)
            .WithMany()
            .HasForeignKey(i => i.InviteeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Owner)
            .WithMany()
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // Avoids cycles or multiple cascade paths.

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
} 