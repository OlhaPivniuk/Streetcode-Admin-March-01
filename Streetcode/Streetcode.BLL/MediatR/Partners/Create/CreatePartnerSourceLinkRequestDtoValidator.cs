using FluentValidation;
using Streetcode.BLL.DTO.Partners.Create;

namespace Streetcode.BLL.MediatR.Partners.Create;

public class CreatePartnerSourceLinkRequestDtoValidator : AbstractValidator<CreatePartnerSourceLinkRequestDto>
{
    private const int MAXTARGETURL = 225;

    public CreatePartnerSourceLinkRequestDtoValidator()
    {
        RuleFor(dto => dto.LogoType)
            .IsInEnum();

        RuleFor(dto => dto.TargetUrl)
            .Must((dto, targetUrl) => targetUrl is null || targetUrl.Contains(dto.LogoType.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage(dto => string.Format("URL must contain the corresponding social network name - {0}", dto.LogoType.ToString()))
            .MaximumLength(MAXTARGETURL);
    }
}