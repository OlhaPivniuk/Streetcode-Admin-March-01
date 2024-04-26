using FluentValidation;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;

namespace Streetcode.BLL.MediatR.Analytics.StatisticRecord.Create;

public class CreateStatisticRecordRequestDtoValidator : AbstractValidator<CreateStatisticRecordRequestDto>
{
    private const int MAXADDRESSLENGTH = 150;

    public CreateStatisticRecordRequestDtoValidator()
    {
        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);

        RuleFor(dto => dto.StreetcodeCoordinateId)
            .GreaterThan(0);

        RuleFor(dto => dto.Address)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(MAXADDRESSLENGTH);
    }
}
