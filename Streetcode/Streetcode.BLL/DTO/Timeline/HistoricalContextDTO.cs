using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.Dto.Timeline;

public class HistoricalContextDto
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;
}
