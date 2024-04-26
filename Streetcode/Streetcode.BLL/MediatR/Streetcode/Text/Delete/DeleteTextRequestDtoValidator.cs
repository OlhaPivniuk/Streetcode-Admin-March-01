using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Delete;

public class DeleteTextRequestDtoValidator : AbstractValidator<DeleteTextRequestDto>
{
    public DeleteTextRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}
