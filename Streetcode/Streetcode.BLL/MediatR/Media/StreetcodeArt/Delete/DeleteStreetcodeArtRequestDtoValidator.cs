using FluentValidation;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.BLL.MediatR.Media.StreetcodeArt.Delete;

public class DeleteStreetcodeArtRequestDtoValidator : AbstractValidator<DeleteStreetcodeArtRequestDto>
{
    public DeleteStreetcodeArtRequestDtoValidator()
    {
        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);
        RuleFor(dto => dto.ArtId)
            .GreaterThan(0);
    }
}
