using FluentValidation;
using Streetcode.BLL.Dto.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Create;

public class CreateArtRequestDtoValidator : AbstractValidator<CreateArtRequestDto>
{
    private const int MAXTITLELENGTH = 150;
    private const int MAXDESCRPTIONLENGTH = 400;

    public CreateArtRequestDtoValidator()
    {
        RuleFor(dto => dto.Title)
            .MaximumLength(MAXTITLELENGTH);

        RuleFor(dto => dto.Description)
            .MaximumLength(MAXDESCRPTIONLENGTH);

        RuleFor(dto => dto.ImageId)
            .GreaterThan(0);

        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);
    }
}