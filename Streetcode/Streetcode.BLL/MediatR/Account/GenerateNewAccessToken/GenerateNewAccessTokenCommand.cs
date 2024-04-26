using MediatR;
using Streetcode.BLL.DTO.Account;
using FluentResults;

namespace Streetcode.BLL.MediatR.Account.GenerateNewAccessToken;

public record GenerateNewAccessTokenCommand(TokenModelDto TokenModel) : IRequest<Result<AuthenticationResponseDto>>;
