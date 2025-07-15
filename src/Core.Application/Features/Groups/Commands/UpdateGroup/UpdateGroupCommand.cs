using Core.Application.DTOs;
using MediatR;
using System;

namespace Core.Application.Features.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand(Guid GroupId, Guid AdminId, UpdateGroupDto Dto) : IRequest; 