using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Partners.Delete;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.BLL.MediatR.Partners.GetAllPartnerShort;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Partners;

[Authorize(Roles = "Admin")]
public class PartnersController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllPartnersQuery()));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllShort()
    {
        return HandleResult(await Mediator.Send(new GetAllPartnersShortQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetPartnerByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetPartnersByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartnerRequestDto partner)
    {
        return HandleResult(await Mediator.Send(new CreatePartnerCommand(partner)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CreatePartnerDto partner)
    {
        return HandleResult(await Mediator.Send(new BLL.MediatR.Partners.Update.UpdatePartnerQuery(partner)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletePartnerRequestDto partner)
    {
        return HandleResult(await Mediator.Send(new DeletePartnerCommand(partner)));
    }
}
