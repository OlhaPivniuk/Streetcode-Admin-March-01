using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.StreetcodeToponym;
using Streetcode.BLL.MediatR.StreetcodeToponym.Delete;
using Streetcode.BLL.MediatR.StreetcodeToponym.Create;
using Microsoft.AspNetCore.Authorization;

namespace Streetcode.WebApi.Controllers.Toponyms;

[Authorize(Roles = "Admin")]
public class StreetcodeToponymController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStreetcodeToponymRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateStreetcodeToponymCommand(createRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStreetcodeToponymRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteStreetcodeToponymCommand(deleteRequest)));
    }
}
