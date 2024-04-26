using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;

public class CreateStreetcodeCoordinateRequestDtoValidator : AbstractValidator<CreateStreetcodeCoordinateRequestDto>
{
    public CreateStreetcodeCoordinateRequestDtoValidator()
    {
        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);

        RuleFor(dto => dto.Latitude)
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90);

        RuleFor(dto => dto.Longtitude)
            .GreaterThanOrEqualTo(-180)
            .LessThanOrEqualTo(180);
    }
}
