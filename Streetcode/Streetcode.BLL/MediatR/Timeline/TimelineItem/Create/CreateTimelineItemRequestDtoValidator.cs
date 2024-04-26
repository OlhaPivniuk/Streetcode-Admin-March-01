using FluentValidation;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public class CreateTimelineItemRequestDtoValidator : AbstractValidator<CreateTimelineItemRequestDto>
{
    public CreateTimelineItemRequestDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .MaximumLength(25)
            .NotEmpty();
        RuleFor(dto => dto.Description)
            .MaximumLength(400)
            .NotEmpty();
        RuleFor(dto => dto.Date)
            .NotNull();
    }
}
