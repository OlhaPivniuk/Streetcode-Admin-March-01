using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Account;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Logout;

public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result<LogoutUserResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILoggerService _logger;
    public LogoutUserHandler(UserManager<ApplicationUser> userManager, ILoggerService loggerService)
    {
        _userManager = userManager;
        _logger = loggerService;
    }

    public async Task<Result<LogoutUserResponseDto>> Handle(LogoutUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.LogoutUserDto;

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return UserByEmailNotFound(request.Email);
        }

        user.RefreshToken = null;

        user.RefreshTokenExpirationDateTime = null;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return FailedToLogout(user);
        }

        LogoutUserResponseDto response = new LogoutUserResponseDto()
        {
            Email = request.Email,
            Text = string.Format("The user '{0}' is logged out successfully", request.Email)
        };

        return Result.Ok(response);
    }

    private Result<LogoutUserResponseDto> UserByEmailNotFound(string email)
    {
        string errorMessage = string.Format(
            ErrorMessages.EntityByPrimaryKeyNotFound,
            email);

        _logger.LogError(email, errorMessage);

        return Result.Fail(errorMessage);
    }

    private Result<LogoutUserResponseDto> FailedToLogout(ApplicationUser user)
    {
        string errorMessage = "Logout failed.";

        _logger.LogError(user, errorMessage);

        return Result.Fail(errorMessage);
    }
}
