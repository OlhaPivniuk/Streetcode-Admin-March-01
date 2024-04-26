using Ardalis.Specification;
using FluentAssertions;
using Xunit;
using static Streetcode.DAL.Specifications.TimelineSpecifications.HistoricalContextSpecs;
using static Streetcode.DAL.Specifications.TimelineSpecifications.TimelineItemSpecs;

namespace Streetcode.XUnitTest.SpecificationTests;

public class SpecificationBuilderExtensions_IncludeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    public void ShouldAddSingleIncludeExpressionInfoToList_GivenIncludeExpression(string contextName)
    {
        // Act
        var spec = new GetByTitleWithHistoricalContextTimelines(contextName);

        // Assert
        spec.IncludeExpressions.Should().ContainSingle();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    public void ShouldAddIncludeExpressionInfoToListWithTypeInclude_GivenIncludeExpression(string contextName)
    {
        // Act
        var spec = new GetByTitleWithHistoricalContextTimelines(contextName);

        // Assert
        spec.IncludeExpressions.Single().Type.Should().Be(IncludeTypeEnum.Include);
    }

    [Fact]
    public void ShouldAppendIncludeExpressionInfoToList_GivenThenIncludeExpression()
    {
        // Act
        var spec = new GetByIdWithHistoricalContextTimelinesAndHistoricalContext(1);

        var includeExpressions = spec.IncludeExpressions.ToList();

        // Assert
        includeExpressions.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldAppendIncludeExpressionInfoToListWithTypeThenInclude_GivenThenIncludeExpression()
    {
        // Act
        var spec = new GetAllWithHistoricalContextTimelinesAndHistoricalContext();

        var includeExpressions = spec.IncludeExpressions.ToList();

        // Assert
        includeExpressions[1].Type.Should().Be(IncludeTypeEnum.ThenInclude);
    }
}
