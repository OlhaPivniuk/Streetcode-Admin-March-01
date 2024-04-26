using FluentValidation;
using Streetcode.BLL.DTO.Media.Art;

namespace Streetcode.BLL.MediatR.Media.Art.Delete;

public class DeleteArtRequestDtoValidator : AbstractValidator<DeleteArtRequestDto>
{
    public DeleteArtRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}
