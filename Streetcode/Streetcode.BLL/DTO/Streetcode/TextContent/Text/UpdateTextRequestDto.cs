namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text;

public sealed record UpdateTextRequestDto(int Id, string? Title, string? TextContent, string? AdditionalText);