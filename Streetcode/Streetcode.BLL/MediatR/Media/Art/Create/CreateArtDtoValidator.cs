using FluentValidation;
using Streetcode.BLL.Dto.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Create;

public class CreateArtDtoValidator : AbstractValidator<CreateArtDto>
{
    private readonly int _maxTitleLength;
    private readonly int _maxDescriptionLength;

    public CreateArtDtoValidator()
    {
        _maxTitleLength = 150;
        _maxDescriptionLength = 400;

        RuleFor(dto => dto.Title)
            .MaximumLength(_maxTitleLength);

        RuleFor(dto => dto.Description)
            .MaximumLength(_maxDescriptionLength);

        RuleFor(dto => dto.ImageId)
            .GreaterThan(0);

        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);
    }
}
