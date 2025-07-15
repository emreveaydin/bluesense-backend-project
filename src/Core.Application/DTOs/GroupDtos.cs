using Core.Domain.Enums;

namespace Core.Application.DTOs;

public record GroupDto(Guid Id, string Name, string? Description, bool IsPublic, Guid OwnerId);

public record CreateGroupDto(string Name, string? Description, bool IsPublic);

public record KickUserDto(Guid UserId);

public record InviteUserToGroupDto(Guid InviteeId);

public record GroupInvitationDto(Guid Id, Guid GroupId, Guid InviteeId, Guid InviterId, InvitationStatus Status, DateTime ExpiresAt);

public record UpdateGroupDto(string? Name, string? Description, bool? IsPublic); 