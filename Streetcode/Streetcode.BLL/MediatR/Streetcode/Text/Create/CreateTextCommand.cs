using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public sealed record CreateTextCommand(CreateTextRequestDto Request) :
    IRequest<Result<CreateTextResponseDto>>;