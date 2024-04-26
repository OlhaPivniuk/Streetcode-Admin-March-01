namespace Streetcode.BLL.DTO.Analytics.StatisticRecord;

public sealed record CreateStatisticRecordResponseDto(int Id, int QrId, int StreetcodeId, int StreetcodeCoordinateId, int Count, string Address);
