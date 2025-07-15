using Core.Domain.Enums;
using System;

namespace Core.Domain.Entities;

public class GroupInvitation : BaseEntity
{
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; } = null!;

    public Guid InviterId { get; set; }
    public virtual User Inviter { get; set; } = null!;

    public Guid InviteeId { get; set; }
    public virtual User Invitee { get; set; } = null!;

    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    
    public DateTime ExpiresAt { get; set; }

    // Private constructor for EF Core
    private GroupInvitation() { }
    
    public GroupInvitation(Guid groupId, Guid inviterId, Guid inviteeId, TimeSpan validity)
    {
        GroupId = groupId;
        InviterId = inviterId;
        InviteeId = inviteeId;
        Status = InvitationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.Add(validity);
    }
} 