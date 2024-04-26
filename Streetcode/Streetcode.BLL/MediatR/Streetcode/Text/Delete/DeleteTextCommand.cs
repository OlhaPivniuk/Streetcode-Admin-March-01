using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Delete;

public sealed record DeleteTextCommand(DeleteTextRequestDto Request) :
    IRequest<Result<DeleteTextResponseDto>>;