namespace Streetcode.BLL.Dto.Media.Art;

public record CreateArtRequestDto(int ImageId, int StreetcodeId, string? Title, string? Description);