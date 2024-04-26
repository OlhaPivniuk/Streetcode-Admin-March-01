using FluentValidation;
using Streetcode.BLL.DTO.Partners.Delete;

namespace Streetcode.BLL.MediatR.Partners.Delete;

public class DeletePartnerRequestDtoValidator : AbstractValidator<DeletePartnerRequestDto>
{
    public DeletePartnerRequestDtoValidator()
    {
        RuleFor(dto => dto.Id)
            .GreaterThan(0);
    }
}