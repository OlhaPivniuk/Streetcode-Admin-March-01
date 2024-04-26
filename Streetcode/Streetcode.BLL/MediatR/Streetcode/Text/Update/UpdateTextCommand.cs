using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update;

public sealed record UpdateTextCommand(UpdateTextRequestDto Request) :
    IRequest<Result<UpdateTextResponseDto>>;
