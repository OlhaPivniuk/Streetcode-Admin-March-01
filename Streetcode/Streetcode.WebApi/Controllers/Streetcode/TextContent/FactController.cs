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

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class FactController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllFactsQuery()));
    }

    [HttpGet("{id:int}")]
    [ServiceFilter(typeof(AsyncValidateEntityExistsAttribute<Fact>))]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetFactByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetFactByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPut]
    public async Task<IActionResult> ReorderFacts([FromBody] ReorderFactRequestDto reorderFactRequestDto)
    {
        return HandleResult(await Mediator.Send(new ReorderFactCommand(reorderFactRequestDto)));
    }

    [HttpPost]
    [ServiceFilter(typeof(ModelStateFilter<Fact>))]

    public async Task<IActionResult> Create([FromBody] CreateFactDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateFactCommand(createRequest)));
    }

    [HttpPut]
    [ServiceFilter(typeof(AsyncValidateEntityExistsAttribute<Fact>))]
    public async Task<IActionResult> Update([FromBody] FactForUpdateDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new FactForUpdateCommand(updateRequest)));
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(AsyncValidateEntityExistsAttribute<Fact>))]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteFactCommand(id)));
    }
}
