using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create;

public class CreateTermRequestDtoValidator : AbstractValidator<CreateTermRequestDto>
{
    private const int MAXTITLE = 50;
    private const int MAXDESCRIPTION = 500;

    public CreateTermRequestDtoValidator()
    {
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
