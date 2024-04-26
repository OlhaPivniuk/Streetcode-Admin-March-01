using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.Dto.Sources;
using Streetcode.BLL.DTO.Sources;
using Microsoft.AspNetCore.Authorization;

namespace Streetcode.WebApi.Controllers.Source;

[Authorize(Roles = "Admin")]
public class SourcesController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllNames()
    {
        return HandleResult(await Mediator.Send(new GetAllCategoryNamesQuery()));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        return HandleResult(await Mediator.Send(new GetAllCategoriesQuery()));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetCategoryByIdQuery(id)));
    }

    [AllowAnonymous]
    [HttpGet("{categoryId:int}&{streetcodeId:int}")]
    public async Task<IActionResult> GetCategoryContentByStreetcodeId([FromRoute] int streetcodeId, [FromRoute] int categoryId)
    {
        return HandleResult(await Mediator.Send(new GetCategoryContentByStreetcodeIdQuery(streetcodeId, categoryId)));
    }

    [AllowAnonymous]
    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetCategoriesByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetCategoriesByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto category)
    {
        return HandleResult(await Mediator.Send(new CreateCategoryCommand(category)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CategoryContentUpdateDto categoryContentUpdateDto)
    {
        return HandleResult(await Mediator.Send(new UpdateCategoryContentCommand(categoryContentUpdateDto)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteCategoryCommand(id)));
    }
}
