namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text;

public sealed record CreateTextRequestDto(int StreetcodeId, string? Title, string? TextContent, string? AdditionalText);