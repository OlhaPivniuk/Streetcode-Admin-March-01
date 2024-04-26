namespace Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

public sealed record CreateStreetcodeCoordinateRequestDto(int StreetcodeId, decimal Latitude, decimal Longtitude);
