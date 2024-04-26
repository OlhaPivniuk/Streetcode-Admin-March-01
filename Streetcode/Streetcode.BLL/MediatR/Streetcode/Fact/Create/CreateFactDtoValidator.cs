using FluentValidation;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public class CreateFactDtoValidator : AbstractValidator<CreateFactDto>
{
    private const int MAXTITLELENGTH = 68;
    private const int MAXFACTCONTENTLENGTH = 600;

    public CreateFactDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(MAXTITLELENGTH);

        RuleFor(dto => dto.FactContent)
            .NotEmpty()
            .MaximumLength(MAXFACTCONTENTLENGTH);

        RuleFor(dto => dto.ImageId)
            .GreaterThan(0);

        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);
    }
}