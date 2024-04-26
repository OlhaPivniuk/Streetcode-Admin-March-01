namespace Streetcode.BLL.Dto.Media.Art;

public record CreateArtResponseDto(int Id, string? Description, string? Title, int ImageId, int StreetcodeId);
