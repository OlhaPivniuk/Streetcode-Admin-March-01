namespace Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

public sealed record CreateStreetcodeCoordinateResponseDto(int Id, int StreetcodeId, decimal Latitude, decimal Longtitude);