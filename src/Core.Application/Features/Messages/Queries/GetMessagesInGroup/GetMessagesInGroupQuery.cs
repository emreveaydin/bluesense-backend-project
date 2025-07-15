using Core.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace Core.Application.Features.Messages.Queries.GetMessagesInGroup;

public class GetMessagesInGroupQuery : IRequest<IEnumerable<MessageDto>>
{
    public Guid GroupId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public GetMessagesInGroupQuery(Guid groupId, int pageNumber, int pageSize)
    {
        GroupId = groupId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
} 