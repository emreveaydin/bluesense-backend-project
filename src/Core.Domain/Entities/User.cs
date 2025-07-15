namespace Core.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public byte[] PasswordHash { get; private set; } = null!;
    public byte[] PasswordSalt { get; private set; } = null!;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    
    public virtual ICollection<GroupMember> GroupMemberships { get; private set; } = new List<GroupMember>();

    // Private constructor for EF Core
    private User() { }

    public User(string username, string email, byte[] passwordHash, byte[] passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public void SetRefreshToken(string? refreshToken, DateTime? expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }
} 