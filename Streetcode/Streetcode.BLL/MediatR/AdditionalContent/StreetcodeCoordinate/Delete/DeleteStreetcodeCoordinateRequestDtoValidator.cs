using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Delete;

public class DeleteStreetcodeCoordinateRequestDtoValidator : AbstractValidator<DeleteStreetcodeCoordinateRequestDto>
{
    public DeleteStreetcodeCoordinateRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}
