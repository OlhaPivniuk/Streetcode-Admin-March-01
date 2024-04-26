using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create;

public sealed record CreateTermCommand(CreateTermRequestDto Request) :
    IRequest<Result<CreateTermResponseDto>>;