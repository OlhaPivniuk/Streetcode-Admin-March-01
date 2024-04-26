using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Update
{
    public record UpdateVideoCommand(UpdateVideoRequestDto Request) : IRequest<Result<UpdateVideoResponseDto>>;
}
