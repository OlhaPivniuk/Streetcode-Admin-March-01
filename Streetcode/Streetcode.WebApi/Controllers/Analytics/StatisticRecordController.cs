using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Create;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;

namespace Streetcode.WebApi.Controllers.Analytics;

[Authorize(Roles = "Admin")]
public class StatisticRecordController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStatisticRecordRequestDto createRequest)
    {
        return HandleResult(await Mediator.Send(new CreateStatisticRecordCommand(createRequest)));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStatisticRecordRequestDto deleteRequest)
    {
        return HandleResult(await Mediator.Send(new DeleteStatisticRecordCommand(deleteRequest)));
    }
}