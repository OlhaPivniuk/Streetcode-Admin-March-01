using Ardalis.Specification;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Specifications.TimelineSpecifications;

public static class TimelineItemSpecs
{
    public class GetById :
        Specification<TimelineItem>,
        ISingleResultSpecification<TimelineItem>
    {
        public GetById(int id)
        {
            Query
                .Where(t => t.Id == id);
        }
    }

    public class GetByIdWithHistoricalContextTimelinesAndHistoricalContext :
        Specification<TimelineItem>,
        ISingleResultSpecification<TimelineItem>
    {
        public GetByIdWithHistoricalContextTimelinesAndHistoricalContext(int id)
        {
            Query
                .Where(t => t.Id == id)
                    .Include(t => t.HistoricalContextTimelines)
                        .ThenInclude(hct => hct.HistoricalContext);
        }
    }

    public class GetAllWithHistoricalContextTimelinesAndHistoricalContext : Specification<TimelineItem>
    {
        public GetAllWithHistoricalContextTimelinesAndHistoricalContext()
        {
            Query
                .Include(t => t.HistoricalContextTimelines)
                    .ThenInclude(hct => hct.HistoricalContext);
        }
    }
}
