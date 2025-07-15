using Core.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace Core.Application.Features.Messages.Queries.SearchMessagesInGroup;

public record SearchMessagesInGroupQuery(Guid GroupId, string SearchText, int PageNumber, int PageSize) : IRequest<IEnumerable<MessageDto>>; 