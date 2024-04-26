using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public record UpdateTimelineItemCommand(UpdateTimelineItemRequestDto UpdateTimelineItemDto) :
    IRequest<Result<TimelineItemDto>>;
