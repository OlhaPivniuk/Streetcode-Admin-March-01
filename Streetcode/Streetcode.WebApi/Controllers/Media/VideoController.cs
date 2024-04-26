using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.MediatR.Media.Video.Delete;
using Streetcode.BLL.MediatR.Media.Video.GetAll;
using Streetcode.BLL.MediatR.Media.Video.GetById;
using Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Media.Video.Update;

namespace Streetcode.WebApi.Controllers.Media;

[Authorize(Roles = "Admin")]
public class VideoController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllVideosQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetVideoByStreetcodeIdQuery(streetcodeId)));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetVideoByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVideoRequestDto request)
    {
        return HandleResult(await Mediator.Send(new CreateVideoCommand(request)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateVideoRequestDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateVideoCommand(updateRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteVideoRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteVideoCommand(deleteRequest)));
    }
}
