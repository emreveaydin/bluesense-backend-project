using Core.Application.DTOs;
using MediatR;
using System;

namespace Core.Application.Features.Groups.Commands.InviteUserToGroup;

public record InviteUserToGroupCommand(Guid GroupId, Guid InviteeId, Guid InviterId) : IRequest<GroupInvitationDto>; 