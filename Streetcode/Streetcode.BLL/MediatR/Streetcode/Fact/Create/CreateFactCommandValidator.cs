using FluentValidation;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public class CreateFactCommandValidator : AbstractValidator<CreateFactCommand>
{
    private readonly int _maxTitleLength;
    private readonly int _maxFactContentLength;

    public CreateFactCommandValidator()
    {
        _maxTitleLength = 68;
        _maxFactContentLength = 600;

        RuleFor(command => command.Request.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(_maxTitleLength)
            .WithMessage($"Title length should not be longer than {_maxTitleLength} symbols.");

        RuleFor(command => command.Request.FactContent)
            .NotEmpty()
            .WithMessage("Fact content is required.")
            .MaximumLength(_maxFactContentLength)
            .WithMessage($"Fact content length should not be longer than {_maxTitleLength} symbols.");

        RuleFor(command => command.Request.ImageId)
            .GreaterThan(0)
            .WithMessage("Image id should be greater than 0.");

        RuleFor(command => command.Request.StreetcodeId)
            .GreaterThan(0)
            .WithMessage("Streetcode id should be greater than 0.");
    }
}
