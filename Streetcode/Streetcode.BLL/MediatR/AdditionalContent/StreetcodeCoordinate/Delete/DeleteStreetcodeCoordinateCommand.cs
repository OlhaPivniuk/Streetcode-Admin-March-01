using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Delete;

public sealed record DeleteStreetcodeCoordinateCommand(DeleteStreetcodeCoordinateRequestDto Request) :
    IRequest<Result<DeleteStreetcodeCoordinateResponseDto>>;