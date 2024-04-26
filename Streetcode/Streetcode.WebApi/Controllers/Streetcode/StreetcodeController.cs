using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Streetcode;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllShort;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetCount;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.Dto.AdditionalContent.Filter;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetShortById;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllStreetcodesMainPage;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
using Streetcode.BLL.DTO.Streetcode;
using Microsoft.AspNetCore.Authorization;

namespace Streetcode.WebApi.Controllers.Streetcode;

[Authorize(Roles = "Admin")]
public class StreetcodeController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStreetcodesRequestDto request)
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesQuery(request)));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllShort()
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesShortQuery()));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllMainPage()
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesMainPageQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetShortById(int id)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeShortByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetByFilter([FromQuery] StreetcodeFilterRequestDto request)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeByFilterQuery(request)));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllCatalog([FromQuery] int page, [FromQuery] int count)
    {
        return HandleResult(await Mediator.Send(new GetAllStreetcodesCatalogQuery(page, count)));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        return HandleResult(await Mediator.Send(new GetStreetcodesCountQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{url}")]
    public async Task<IActionResult> GetByTransliterationUrl([FromRoute] string url)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeByTransliterationUrlQuery(url)));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetStreetcodeByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStreetcodeRequestDto request)
    {
        return HandleResult(await Mediator.Send(new CreateStreetcodeCommand(request)));
    }
}
