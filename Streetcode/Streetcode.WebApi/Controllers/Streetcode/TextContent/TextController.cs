using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.BLL.MediatR.Streetcode.Text.Delete;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.MediatR.Streetcode.Text.Preview;
using Streetcode.BLL.MediatR.Streetcode.Text.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

[Authorize(Roles = "Admin")]
public class TextController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTextsQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTextByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTextByStreetcodeIdQuery(streetcodeId)));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetParsedText([FromQuery] string text)
    {
        return HandleResult(await Mediator.Send(new GetParsedTextForAdminPreviewCommand(text)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTextRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateTextCommand(createRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTextRequestDto updateRequest)
    {
        return HandleResult(await Mediator.Send(new UpdateTextCommand(updateRequest)));
    }

    [HttpPut]
    public async Task<IActionResult> Preview([FromBody] PreviewTextRequestDto previewRequest)
    {
        return HandleResult(await Mediator.Send(new PreviewTextQuery(previewRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteTextRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteTextCommand(deleteRequest)));
    }
}