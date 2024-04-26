using FluentResults;
using MediatR;
using Streetcode.BLL.Dto.Account;
using Streetcode.BLL.DTO.Account;

namespace Streetcode.BLL.MediatR.Account.Register;

public record RegisterUserCommand(RegisterUserDto RegisterUserDto) : IRequest<Result<AuthenticationResponseDto>>;
