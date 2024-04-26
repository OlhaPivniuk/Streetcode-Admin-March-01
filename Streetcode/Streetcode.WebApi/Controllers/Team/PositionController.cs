using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Team;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.MediatR.Team.Position.GetAll;

namespace Streetcode.WebApi.Controllers.Team
{
    [Authorize(Roles = "Admin")]
    public class PositionController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllPositionsQuery()));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PositionDto position)
        {
            return HandleResult(await Mediator.Send(new CreatePositionQuery(position)));
        }
    }
}
