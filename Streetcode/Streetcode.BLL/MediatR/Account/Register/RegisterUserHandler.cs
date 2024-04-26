using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Dto.Account;
using Streetcode.BLL.DTO.Account;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthenticationResponseDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;

    public RegisterUserHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, ILoggerService logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponseDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.RegisterUserDto;

        if(await IsEmailAlreadyInUse(request.Email!))
        {
            return EmailIsAlreadyRegistered(request);
        }

        // Create user
        ApplicationUser user = new()
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName!,
            LastName = request.LastName!,
        };

        IdentityResult creatingUserResult = await _userManager.CreateAsync(user, request.Password);

        if (!creatingUserResult.Succeeded)
        {
            return FailedToRegister(creatingUserResult);
        }

        // sign-in
        await _signInManager.SignInAsync(user, isPersistent: false);

        // add user role
        IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, UserRole.USER);

        if (!addingRoleResult.Succeeded)
        {
            return FailedToAssignRole(addingRoleResult);
        }

        var claims = await _tokenService.GetUserClaimsAsync(user);

        var response = _tokenService.GenerateJWTToken(user, claims);

        user.RefreshToken = response.RefreshToken;

        user.RefreshTokenExpirationDateTime = response.RefreshTokenExpirationDateTime;

        await _userManager.UpdateAsync(user);

        return Result.Ok(response);
    }

    private Result<AuthenticationResponseDto> EmailIsAlreadyRegistered(RegisterUserDto request)
    {
        string errorMessage = "A user with this email is already registered.";
        _logger.LogError(request, errorMessage);

        return Result.Fail(errorMessage);
    }

    private async Task<bool> IsEmailAlreadyInUse(string email)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        return user is not null;
    }

    private static Result<AuthenticationResponseDto> FailedToAssignRole(IdentityResult addingRoleResult)
    {
        string errorMessage = string.Join(
            Environment.NewLine,
            addingRoleResult.Errors.Select(e => e.Description));

        return Result.Fail(errorMessage);
    }

    private static Result<AuthenticationResponseDto> FailedToRegister(IdentityResult creatingUserResult)
    {
        string errorMessage = string.Join(
            Environment.NewLine,
            creatingUserResult.Errors.Select(e => e.Description));

        return Result.Fail(errorMessage);
    }
}
