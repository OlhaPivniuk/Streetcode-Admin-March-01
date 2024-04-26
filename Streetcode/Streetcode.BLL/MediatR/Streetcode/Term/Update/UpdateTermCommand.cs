using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update;

public sealed record UpdateTermCommand(UpdateTermRequestDto Request) :
    IRequest<Result<UpdateTermResponseDto>>;