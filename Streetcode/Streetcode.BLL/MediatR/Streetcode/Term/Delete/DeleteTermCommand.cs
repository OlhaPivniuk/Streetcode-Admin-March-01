using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Delete;

public sealed record DeleteTermCommand(DeleteTermRequestDto Request) :
    IRequest<Result<DeleteTermResponseDto>>;