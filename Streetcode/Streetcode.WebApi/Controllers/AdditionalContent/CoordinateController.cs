using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.AdditionalContent;

[AllowAnonymous]
public class CoordinateController : BaseApiController
{
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetCoordinatesByStreetcodeIdQuery(streetcodeId)));
    }
}