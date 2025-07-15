namespace Core.Application.DTOs;

public record MessageDto(
    Guid Id,
    string Content,
    UserDto Sender,
    Guid GroupId,
    DateTime CreatedAt,
    bool IsEdited,
    string? FileUrl
);

public record CreateMessageDto(string Content, Guid GroupId, string? FileUrl = null);

public record EditMessageDto(string NewContent); 