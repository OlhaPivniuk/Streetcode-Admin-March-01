using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Dto.Timeline;

public record UpdateTimelineItemRequestDto(
    int Id,
    int StreetcodeId,
    string Title,
    string Description,
    DateTime Date,
    DateViewPattern DateViewPattern,
    string? HistoricalContext);
