namespace Core.Domain.Entities;

public class Message : BaseEntity
{
    public string Content { get; private set; } = null!;
    public Guid SenderId { get; private set; }
    public virtual User Sender { get; private set; } = null!;
    public Guid GroupId { get; private set; }
    public virtual Group Group { get; private set; } = null!;

    public bool IsEdited { get; private set; }
    public bool IsDeleted { get; private set; } // Soft delete flag
    public string? FileUrl { get; private set; }

    // Private constructor for EF Core/Mongo
    private Message() { }

    public Message(string content, Guid senderId, Guid groupId, string? fileUrl = null)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("Message content or file URL must be provided.");
        if (senderId == Guid.Empty)
            throw new ArgumentException("Sender ID cannot be empty.", nameof(senderId));
        if (groupId == Guid.Empty)
            throw new ArgumentException("Group ID cannot be empty.", nameof(groupId));

        Content = content;
        SenderId = senderId;
        GroupId = groupId;
        FileUrl = fileUrl;
    }

    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("New content cannot be empty.", nameof(newContent));

        Content = newContent;
        IsEdited = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
} 