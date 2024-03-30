using FluentValidation;
using Streetcode.BLL.Resources.Errors.ValidationErrors;

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
            .MaximumLength(_maxTitleLength);

        RuleFor(command => command.Request.FactContent)
            .NotEmpty()
            .MaximumLength(_maxFactContentLength);

        RuleFor(command => command.Request.ImageId)
            .GreaterThan(0);

        RuleFor(command => command.Request.StreetcodeId)
            .GreaterThan(0);
    }
}
