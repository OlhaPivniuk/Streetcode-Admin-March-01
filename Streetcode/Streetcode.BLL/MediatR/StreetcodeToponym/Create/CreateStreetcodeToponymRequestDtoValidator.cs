using FluentValidation;
using Streetcode.BLL.DTO.StreetcodeToponym;

namespace Streetcode.BLL.MediatR.StreetcodeToponym.Create;

public class CreateStreetcodeToponymRequestDtoValidator : AbstractValidator<CreateStreetcodeToponymRequestDto>
{
    public CreateStreetcodeToponymRequestDtoValidator()
    {
        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);

        RuleFor(dto => dto.ToponymId)
            .GreaterThan(0);
    }
}