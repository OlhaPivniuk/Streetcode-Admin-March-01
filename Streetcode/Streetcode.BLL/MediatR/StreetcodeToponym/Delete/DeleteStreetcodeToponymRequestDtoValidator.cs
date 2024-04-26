using FluentValidation;
using Streetcode.BLL.DTO.StreetcodeToponym;

namespace Streetcode.BLL.MediatR.StreetcodeToponym.Delete;

public class DeleteStreetcodeToponymRequestDtoValidator : AbstractValidator<DeleteStreetcodeToponymRequestDto>
{
    public DeleteStreetcodeToponymRequestDtoValidator()
    {
        RuleFor(dto => dto.StreetcodeId)
            .GreaterThan(0);

        RuleFor(dto => dto.ToponymId)
            .GreaterThan(0);
    }
}