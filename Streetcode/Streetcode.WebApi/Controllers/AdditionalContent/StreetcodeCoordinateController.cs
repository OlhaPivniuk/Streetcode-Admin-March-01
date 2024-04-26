using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;
using Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Delete;

namespace Streetcode.WebApi.Controllers.AdditionalContent;

[Authorize(Roles = "Admin")]
public class StreetcodeCoordinateController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStreetcodeCoordinateRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateStreetcodeCoordinateCommand(createRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStreetcodeCoordinateRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteStreetcodeCoordinateCommand(deleteRequest)));
    }
}
