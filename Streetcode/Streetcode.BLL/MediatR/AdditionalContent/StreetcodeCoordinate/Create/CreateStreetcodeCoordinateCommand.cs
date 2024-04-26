using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;

public sealed record CreateStreetcodeCoordinateCommand(CreateStreetcodeCoordinateRequestDto Request) :
    IRequest<Result<CreateStreetcodeCoordinateResponseDto>>;