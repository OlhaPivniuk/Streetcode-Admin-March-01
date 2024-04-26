using Ardalis.Specification;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Specifications.TimelineSpecifications;

public static class HistoricalContextSpecs
{
    public class GetByTitleWithHistoricalContextTimelines : Specification<HistoricalContext>,
        ISingleResultSpecification<HistoricalContext>
    {
        public GetByTitleWithHistoricalContextTimelines(string title)
        {
            Query
                .Where(hc => hc.Title == title)
                .Include(hc => hc.HistoricalContextTimelines);
        }
    }
}