using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public record CreateTimelineItemCommand(CreateTimelineItemRequestDto CreateTimelineItemRequestDto) : IRequest<Result<TimelineItemDto>>;
