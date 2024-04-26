using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Delete;

public class DeleteTermRequestDtoValidator : AbstractValidator<DeleteTermRequestDto>
{
    public DeleteTermRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}