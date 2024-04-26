using FluentValidation;
using Streetcode.BLL.Dto.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public class UpdateTimelineItemRequestDtoValidator : AbstractValidator<UpdateTimelineItemRequestDto>
{
    private const int MAXTITLELENGTH = 26;
    private const int MAXDESCRIPTIONLENGTH = 400;
    private const int MAXCONTEXTLENGTH = 50;

    public UpdateTimelineItemRequestDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .MaximumLength(MAXTITLELENGTH);

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .MaximumLength(MAXDESCRIPTIONLENGTH);

        RuleFor(dto => dto.HistoricalContext)
            .MaximumLength(MAXCONTEXTLENGTH);

        RuleFor(dto => dto.DateViewPattern)
            .IsInEnum();
    }
}
