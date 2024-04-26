using System.Security.Claims;
using MediatR;
using Streetcode.BLL.DTO.Account;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.GenerateNewAccessToken;

public sealed class GenerateNewAccessTokenHandler : IRequestHandler<GenerateNewAccessTokenCommand, Result<AuthenticationResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;
    public GenerateNewAccessTokenHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, ILoggerService logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponseDto>> Handle(GenerateNewAccessTokenCommand command, CancellationToken cancellationToken)
    {
        var request = command.TokenModel;

        string accessToken = request.Token;

        string refreshToken = request.RefreshToken;

        ClaimsPrincipal principals = _tokenService.GetPrincipalFromJwtToken(accessToken)!;

        if (principals is null)
        {
            return InvalidJwtToken(accessToken);
        }

        string email = principals.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationDateTime <= DateTime.UtcNow)
        {
            return InvalidRefreshToken(refreshToken);
        }

        var claims = await _tokenService.GetUserClaimsAsync(user);

        var response = _tokenService.GenerateJWTToken(user, claims);

        user.RefreshToken = response.RefreshToken;

        user.RefreshTokenExpirationDateTime = response.RefreshTokenExpirationDateTime;

        await _userManager.UpdateAsync(user);

        return Result.Ok(response);
    }

    private Result<AuthenticationResponseDto> InvalidRefreshToken(string refreshToken)
    {
        var errorMessage = string.Join(
            Environment.NewLine,
            "Invalid Refresh Token");

        _logger.LogError(refreshToken, errorMessage);

        return Result.Fail(errorMessage);
    }

    private Result<AuthenticationResponseDto> InvalidJwtToken(string jwtToken)
    {
        var errorMessage = string.Join(
            Environment.NewLine,
            "Invalid Jwt Token");

        _logger.LogError(jwtToken, errorMessage);

        return Result.Fail(errorMessage);
    }
}
