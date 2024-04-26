using Ardalis.Specification;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Specifications.TimelineSpecifications;

public static class HistoricalContextTimelineSpecs
{
    public class GetAllByHistoricalContextIdAndTimelineId : Specification<HistoricalContextTimeline>
    {
        public GetAllByHistoricalContextIdAndTimelineId(int historicalContextId, int TimelineId)
        {
            Query
                .Where(hct => hct.HistoricalContextId == historicalContextId
                    && hct.TimelineId == TimelineId);
        }
    }

    public class GetByHistoricalContextIdAndTimelineId : Specification<HistoricalContextTimeline>,
        ISingleResultSpecification<HistoricalContextTimeline>
    {
        public GetByHistoricalContextIdAndTimelineId(int historicalContextId, int TimelineId)
        {
            Query
            .Where(hct => hct.HistoricalContextId == historicalContextId
                    && hct.TimelineId == TimelineId);
        }
    }
}