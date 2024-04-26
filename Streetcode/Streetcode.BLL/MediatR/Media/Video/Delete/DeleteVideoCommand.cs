using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.BLL.MediatR.Media.Video.Delete
{
    public sealed record DeleteVideoCommand(DeleteVideoRequestDto Request) :
    IRequest<Result<DeleteVideoResponseDto>>;
}
