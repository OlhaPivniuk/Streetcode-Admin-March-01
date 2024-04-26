namespace Streetcode.BLL.DTO.Analytics.StatisticRecord;

public sealed record CreateStatisticRecordRequestDto(int StreetcodeId, int StreetcodeCoordinateId, string Address);
