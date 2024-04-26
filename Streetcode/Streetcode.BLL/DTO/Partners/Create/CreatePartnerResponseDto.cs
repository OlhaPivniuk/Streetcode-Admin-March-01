using Streetcode.BLL.Dto.Streetcode;

namespace Streetcode.BLL.DTO.Partners.Create;

public sealed record CreatePartnerResponseDto(
    int Id,
    string Title,
    int LogoId,
    bool IsKeyPartner,
    bool IsVisibleEverywhere,
    string? TargetUrl,
    string? UrlTitle,
    string? Description,
    List<CreatePartnerSourceLinkResponseDto> PartnerSourceLinks,
    List<StreetcodeShortDto> Streetcodes);
