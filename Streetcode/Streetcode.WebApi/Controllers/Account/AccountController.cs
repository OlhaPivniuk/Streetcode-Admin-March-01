using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Dto.Account;
using Streetcode.BLL.DTO.Account;
using Streetcode.BLL.MediatR.Account.GenerateNewAccessToken;
using Streetcode.BLL.MediatR.Account.Login;
using Streetcode.BLL.MediatR.Account.Logout;
using Streetcode.BLL.MediatR.Account.Register;

namespace Streetcode.WebApi.Controllers.Account
{
    [AllowAnonymous]
    public class AccountController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerRequest)
        {
            return HandleResult(await Mediator.Send(new RegisterUserCommand(registerRequest)));
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            return HandleResult(await Mediator.Send(new LoginUserCommand(loginUserDto)));
        }

        [HttpPost]
        public async Task<IActionResult> Logout(LogoutUserDto logoutUser)
        {
            return HandleResult(await Mediator.Send(new LogoutUserCommand(logoutUser)));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModelDto tokenModel)
        {
            return HandleResult(await Mediator.Send(new GenerateNewAccessTokenCommand(tokenModel)));
        }
    }
}
