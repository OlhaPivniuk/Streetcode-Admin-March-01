using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.StreetcodeToponym;

namespace Streetcode.BLL.MediatR.StreetcodeToponym.Create;

public sealed record CreateStreetcodeToponymCommand(CreateStreetcodeToponymRequestDto Request) :
    IRequest<Result<CreateStreetcodeToponymResponseDto>>;