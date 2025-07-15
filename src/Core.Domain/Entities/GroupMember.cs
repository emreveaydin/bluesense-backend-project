using Core.Domain.Enums;
using System;

namespace Core.Domain.Entities;

public class GroupMember
{
    public Guid GroupId { get; set; }
    public virtual Group Group { get; set; } = null!;

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public GroupRole Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
} 