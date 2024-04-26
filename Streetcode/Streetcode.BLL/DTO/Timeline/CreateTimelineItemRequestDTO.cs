using System.Diagnostics.CodeAnalysis;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Timeline;

public class CreateTimelineItemRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateViewPattern DateViewPattern { get; set; }
    [AllowNull]
    public HistoricalContextDto? HistoricalContext { get; set; }
    public int StreetcodeId { get; set; }
}