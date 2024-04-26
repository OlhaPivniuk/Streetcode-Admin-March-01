using FluentValidation;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update;

public class UpdateFactDtoValidator : AbstractValidator<UpdateFactDto>
{
    private const int MAXTITLELENGTH = 68;
    private const int MAXFACTCONTENTLENGTH = 600;

    public UpdateFactDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);

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