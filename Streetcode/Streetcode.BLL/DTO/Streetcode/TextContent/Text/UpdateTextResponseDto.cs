namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text;

public sealed record UpdateTextResponseDto(int Id, int StreetcodeId, string? Title, string? TextContent, string? AdditionalText);