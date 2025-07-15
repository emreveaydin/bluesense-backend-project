namespace Core.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsPublic { get; private set; }
    public Guid OwnerId { get; private set; }
    public virtual User Owner { get; private set; } = null!;

    public virtual ICollection<GroupMember> GroupMembers { get; private set; } = new List<GroupMember>();

    // Private constructor for EF Core
    private Group() { }

    public Group(string name, Guid ownerId, string? description = null, bool isPublic = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty.", nameof(name));
        if (ownerId == Guid.Empty)
            throw new ArgumentException("Owner ID cannot be empty.", nameof(ownerId));

        Name = name;
        OwnerId = ownerId;
        Description = description;
        IsPublic = isPublic;
    }

    public void UpdateDetails(string? name, string? description, bool? isPublic)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        if (description != null)
        {
            Description = description;
        }

        if (isPublic.HasValue)
        {
            IsPublic = isPublic.Value;
        }
    }
} 