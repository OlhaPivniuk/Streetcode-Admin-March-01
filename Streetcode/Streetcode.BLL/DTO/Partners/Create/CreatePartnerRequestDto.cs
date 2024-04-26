namespace Streetcode.BLL.DTO.Partners.Create;

public sealed record CreatePartnerRequestDto(
    string Title,
    int LogoId,
    bool IsKeyPartner,
    bool IsVisibleEverywhere,
    string? TargetUrl,
    string? UrlTitle,
    string? Description,
    List<CreatePartnerSourceLinkRequestDto> PartnerSourceLinks,
    List<int> Streetcodes);
