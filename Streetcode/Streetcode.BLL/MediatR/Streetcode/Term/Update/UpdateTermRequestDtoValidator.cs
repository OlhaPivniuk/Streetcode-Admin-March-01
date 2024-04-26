using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update;

public class UpdateTermRequestDtoValidator : AbstractValidator<UpdateTermRequestDto>
{
    private const int MAXTITLE = 50;
    private const int MAXDESCRIPTION = 500;

    public UpdateTermRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);

        RuleFor(dto => dto.Title)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(MAXTITLE);

        RuleFor(dto => dto.Description)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(MAXDESCRIPTION);
    }
}
