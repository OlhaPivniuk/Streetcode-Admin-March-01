﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent
{
    [AllowAnonymous]
    public class RelatedTermController : BaseApiController
    {
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByTermId([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetAllRelatedTermsByTermIdQuery(id)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RelatedTermDto relatedTerm)
        {
            return HandleResult(await Mediator.Send(new CreateRelatedTermCommand(relatedTerm)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RelatedTermDto relatedTerm)
        {
            return HandleResult(await Mediator.Send(new UpdateRelatedTermCommand(id, relatedTerm)));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{word}")]
        public async Task<IActionResult> Delete([FromRoute] string word)
        {
            return HandleResult(await Mediator.Send(new DeleteRelatedTermCommand(word)));
        }
    }
}
