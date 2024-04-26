using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Partners.Create;

public sealed record CreatePartnerSourceLinkResponseDto(int id, LogoType LogoType, string? TargetUrl, int PartnerId);