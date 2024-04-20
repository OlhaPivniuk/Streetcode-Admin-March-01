﻿namespace Streetcode.BLL.DTO.Partners.Create;

public sealed record CreatePartnerResponseDto(
    string Title,
    int LogoId,
    bool IsKeyPartner,
    bool IsVisibleEverywhere,
    string? TargetUrl,
    string? UrlTitle,
    string? Description,
    List<int> PartnerSourceLinks,
    List<int> Streetcodes);
