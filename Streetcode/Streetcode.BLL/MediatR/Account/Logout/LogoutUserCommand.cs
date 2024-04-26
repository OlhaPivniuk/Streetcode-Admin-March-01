using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Account;

namespace Streetcode.BLL.MediatR.Account.Logout;

public record LogoutUserCommand(LogoutUserDto LogoutUserDto) : IRequest<Result<LogoutUserResponseDto>>;