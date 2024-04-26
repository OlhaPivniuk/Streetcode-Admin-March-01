using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Partners.Create;

public sealed record CreatePartnerSourceLinkRequestDto(LogoType LogoType, string? TargetUrl);
