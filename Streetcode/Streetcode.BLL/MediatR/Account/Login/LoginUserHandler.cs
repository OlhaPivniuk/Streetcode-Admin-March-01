using System.IdentityModel.Tokens.Jwt;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Account;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Login;

public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<AuthenticationResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;
    public LoginUserHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, ILoggerService logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponseDto>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.LoginUserDto;

        // sign-in
        var result = await _signInManager.PasswordSignInAsync(
            request.Email,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return FailedToSign(result);
        }

        ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return FailedToFindByEmail(request);
        }

        // sign-in
        await _signInManager.SignInAsync(user, isPersistent: false);

        var claims = await _tokenService.GetUserClaimsAsync(user);

        var response = _tokenService.GenerateJWTToken(user, claims);

        user.RefreshToken = response.RefreshToken;

        user.RefreshTokenExpirationDateTime = response.RefreshTokenExpirationDateTime;

        await _userManager.UpdateAsync(user);

        return Result.Ok(response);
    }

    private Result<AuthenticationResponseDto> FailedToSign(SignInResult result)
    {
        var errorMessage = string.Join(
            Environment.NewLine,
            result.Succeeded.ToString());

        _logger.LogError(result, errorMessage);

        return Result.Fail(errorMessage);
    }

    private Result<AuthenticationResponseDto> FailedToFindByEmail(LoginUserDto request)
    {
        var errorMessage = string.Format(
            ErrorMessages.EntityByPrimaryKeyNotFound,
            request.Email);

        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }
}
