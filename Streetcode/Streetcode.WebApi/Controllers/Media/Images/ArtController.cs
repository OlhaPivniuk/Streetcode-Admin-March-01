using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Media.Images;

[Authorize(Roles = "Admin")]
public class ArtController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllArtsQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetArtByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetArtsByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateArt([FromBody] CreateArtRequestDto art)
    {
        return HandleResult(await Mediator.Send(new CreateArtCommand(art)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteArtRequestDto deleteArtRequestDto)
    {
        return HandleResult(await Mediator.Send(new DeleteArtCommand(deleteArtRequestDto)));
    }
}
