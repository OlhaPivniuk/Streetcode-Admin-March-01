using AutoMapper;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public class TimelineItemProfile : Profile
{
    public TimelineItemProfile()
    {
        CreateMap<TimelineItem, TimelineItemDto>().ReverseMap();
        CreateMap<UpdateTimelineItemRequestDto, TimelineItem>();
        CreateMap<CreateHistoricalContextRequestDto, TimelineItem>();
    }
}
