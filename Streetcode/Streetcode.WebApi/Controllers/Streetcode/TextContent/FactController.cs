using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.BLL.ActionFilters;
using Microsoft.AspNetCore.Authorization;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

[Authorize(Roles = "Admin")]
[ServiceFilter(typeof(AsyncValidateEntityExistsFilter<Fact>))]
public class FactController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllFactsQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetFactByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetFactByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFactDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateFactCommand(createRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateFactDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateFactCommand(updateRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> ReorderFacts([FromBody] ReorderFactRequestDto reorderFactRequestDto)
    {
        return HandleResult(await Mediator.Send(new ReorderFactCommand(reorderFactRequestDto)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteFactCommand(id)));
    }
}
