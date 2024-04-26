using FluentValidation;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;

namespace Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;

public class DeleteStatisticRecordRequestDtoValidator : AbstractValidator<DeleteStatisticRecordRequestDto>
{
    public DeleteStatisticRecordRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}