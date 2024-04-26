using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Preview;

public sealed record PreviewTextQuery(PreviewTextRequestDto Request) :
    IRequest<Result<PreviewTextResponseDto>>;