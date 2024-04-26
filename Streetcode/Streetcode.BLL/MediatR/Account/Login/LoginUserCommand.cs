using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Account;

namespace Streetcode.BLL.MediatR.Account.Login;

public record LoginUserCommand(LoginUserDto LoginUserDto) : IRequest<Result<AuthenticationResponseDto>>;