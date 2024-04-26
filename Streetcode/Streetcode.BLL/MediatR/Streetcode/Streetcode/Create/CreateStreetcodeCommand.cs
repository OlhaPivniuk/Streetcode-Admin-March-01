using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Create
{
    public sealed record CreateStreetcodeCommand(CreateStreetcodeRequestDto Request)
     : IRequest<Result<CreateStreetcodeResponseDto>>;
}
