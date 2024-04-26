using FluentValidation;
using Streetcode.BLL.Dto.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateCategoryRequestDtoValidator : AbstractValidator<CreateCategoryRequestDto>
{
    private const int MAXTITLELENGTH = 100;
    private const int MAXTEXTLENGTH = 4000;

    public CreateCategoryRequestDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .MaximumLength(MAXTITLELENGTH);

        RuleFor(dto => dto.Text)
            .MaximumLength(MAXTEXTLENGTH);

        RuleFor(dto => dto.ImageId)
            .GreaterThan(0);

        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);
    }
}
