using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Payment;
using Streetcode.BLL.MediatR.Payment;

namespace Streetcode.WebApi.Controllers.Payment
{
    [Authorize]
    public class PaymentController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] PaymentDto payment)
        {
            return HandleResult(await Mediator.Send(new CreateInvoiceCommand(payment)));
        }
    }
}
