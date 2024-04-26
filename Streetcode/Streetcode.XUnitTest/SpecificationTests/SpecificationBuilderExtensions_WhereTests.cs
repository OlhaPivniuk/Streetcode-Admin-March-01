using FluentAssertions;
using Xunit;
using static Streetcode.DAL.Specifications.TimelineSpecifications.HistoricalContextSpecs;
using static Streetcode.DAL.Specifications.TimelineSpecifications.HistoricalContextTimelineSpecs;
using static Streetcode.DAL.Specifications.TimelineSpecifications.TimelineItemSpecs;

namespace Streetcode.XUnitTest.SpecificationTests;

public class SpecificationBuilderExtensions_WhereTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    [InlineData(int.MaxValue, int.MinValue)]
    public void ShouldAddOneExpressionToList_GivenOneWhereExpressionWithIntParameters(
        int historicalContextId,
        int TimelineId)
    {
        // Act
        var spec = new GetAllByHistoricalContextIdAndTimelineId(historicalContextId, TimelineId);

        // Assert
        spec.WhereExpressions.Should().ContainSingle();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    public void ShouldAddOneExpressionToList_GivenOneWhereExpressionWithStringParameter(string contextName)
    {
        // Act
        var spec = new GetByTitleWithHistoricalContextTimelines(contextName);

        // Assert
        spec.WhereExpressions.Should().ContainSingle();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void ShouldAddOneExpressionToList_GivenOneWhereExpressionWithIntParameter(int id)
    {
        // Act
        var spec = new GetById(id);

        // Assert
        spec.WhereExpressions.Should().ContainSingle();
    }
}
