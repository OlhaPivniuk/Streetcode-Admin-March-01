using FluentValidation;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

public class UpdateCategoryContentValidator : AbstractValidator<CategoryContentUpdateDto>
{
    private const int MAXTEXTLENGTH = 4000;

    public UpdateCategoryContentValidator()
    {
        RuleFor(dto => dto.Text)
            .NotEmpty()
            .MaximumLength(MAXTEXTLENGTH);

        RuleFor(dto => dto.SourceLinkCategoryId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);
    }
}
