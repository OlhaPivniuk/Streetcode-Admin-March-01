using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.BLL.MediatR.Streetcode.Term.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

[Authorize(Roles = "Admin")]
public class TermController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTermsQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTermByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTermRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateTermCommand(createRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTermRequestDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateTermCommand(updateRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteTermRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteTermCommand(deleteRequest)));
    }
}
