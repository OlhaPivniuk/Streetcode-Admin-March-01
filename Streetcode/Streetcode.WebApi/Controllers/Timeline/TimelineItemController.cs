using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

namespace Streetcode.WebApi.Controllers.Timeline;

[Authorize(Roles = "Admin")]
public class TimelineItemController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTimelineItemsQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimelineItemRequestDto createTimelineItemRequestDto)
    {
        return HandleResult(await Mediator.Send(new CreateTimelineItemCommand(createTimelineItemRequestDto)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTimelineItemRequestDto timeline)
    {
        return HandleResult(await Mediator.Send(new UpdateTimelineItemCommand(timeline)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTimelineItemCommand(id)));
    }
}