using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem;

public class UpdateTimelineItemHandlerTests
{
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateTimelineItemHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetRequestDto("context");
        MockRepositoryWrapperSetup(FAILEDSAVE);
        SetupMockMapper();

        var handler = CreateHandler();
        var command = new UpdateTimelineItemCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    private UpdateTimelineItemHandler CreateHandler()
    {
        return new UpdateTimelineItemHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void MockRepositoryWrapperSetup(int saveResult)
    {
        _mockRepositoryWrapper.Setup(x => x.HistoricalContextRepository
            .GetFirstOrDefaultAsync(
                AnyEntityPredicate<HistoricalContext>(),
                AnyEntityInclude<HistoricalContext>()))
            .ReturnsAsync(GetHistoricalContextEntity());

        _mockRepositoryWrapper.Setup(repo => repo.HistoricalContextRepository
            .Create(It.IsAny<HistoricalContext>()))
            .Returns(GetHistoricalContextEntity());

        _mockRepositoryWrapper.Setup(repo => repo.HistoricalContextTimelineRepository
        .Create(It.IsAny<HistoricalContextTimeline>()))
            .Returns(GetHistoricalContextTimelineEntity());

        _mockRepositoryWrapper.Setup(repo => repo.HistoricalContextTimelineRepository
        .GetFirstOrDefaultAsync(
            AnyEntityPredicate<HistoricalContextTimeline>(),
            AnyEntityInclude<HistoricalContextTimeline>()))
                .ReturnsAsync(GetHistoricalContextTimelineEntity());

        _mockRepositoryWrapper.Setup(x => x.TimelineRepository
            .Update(GetTimelineItemEntity()));

        _mockRepositoryWrapper.Setup(x => x.TimelineRepository
        .GetFirstOrDefaultAsync(
            AnyEntityPredicate<TimelineItemEntity>(),
            AnyEntityInclude<TimelineItemEntity>()))
                .ReturnsAsync(GetTimelineItemEntity());

        _mockRepositoryWrapper.SetupSequence(repo => repo.HistoricalContextTimelineRepository
            .GetAllAsync(
                AnyEntityPredicate<HistoricalContextTimeline>(),
                AnyEntityInclude<HistoricalContextTimeline>()))
                    .ReturnsAsync(GetListHistoricalContextTimelines());

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
    }

    private static IEnumerable<HistoricalContextTimeline> GetListHistoricalContextTimelines()
    {
        return new List<HistoricalContextTimeline>
        {
            new ()
            {
                TimelineId = 1,
                HistoricalContextId = 1,
            },
            new()
            {
                TimelineId = 2,
                HistoricalContextId = 1,
            }
        };
    }

    private static HistoricalContextTimeline GetHistoricalContextTimelineEntity()
    {
        return new HistoricalContextTimeline
        {
            TimelineId = 1,
            HistoricalContextId = 1,
        };
    }

    private static HistoricalContext GetHistoricalContextEntity()
    {
        return new HistoricalContext()
        {
            Id = 1,
            Title = "Title"
        };
    }

    private void SetupMockMapper()
    {
        _mockMapper.Setup(x => x.Map<TimelineItemEntity>(It.IsAny<UpdateTimelineItemRequestDto>()))
            .Returns(GetTimelineItemEntity());

        _mockMapper.Setup(x => x.Map<TimelineItemDto>(It.IsAny<TimelineItemEntity>()))
            .Returns(GetTimelineItemDto());
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static UpdateTimelineItemRequestDto GetRequestDto(string? context)
    {
        return new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "Title",
            Description: "Description",
            Date: new DateTime(2024, 4, 11, 0, 0, 0, DateTimeKind.Utc),
            DateViewPattern: DAL.Enums.DateViewPattern.DateMonthYear,
            HistoricalContext: context);
    }

    private static TimelineItemDto GetTimelineItemDto()
    {
        return new TimelineItemDto
        {
            Id = 1,
            Date = new DateTime(2024, 4, 11, 0, 0, 0, DateTimeKind.Utc),
            DateViewPattern = DAL.Enums.DateViewPattern.DateMonthYear,
            Title = "Title",
            Description = "Description",
            HistoricalContexts = new[]
            {
                new HistoricalContextDto()
                {
                    Id = 1, Title = "Title"
                }
            }
        };
    }

    private static TimelineItemEntity GetTimelineItemEntity()
    {
        return new TimelineItemEntity
        {
            Id = 1,
            Date = DateTime.UtcNow,
            DateViewPattern = DAL.Enums.DateViewPattern.DateMonthYear,
            Title = "Title",
            Description = "Description",
            HistoricalContextTimelines = new List<HistoricalContextTimeline>
            {
                new ()
                {
                    TimelineId = 1,
                    HistoricalContextId = 1,
                }
            },
        };
    }
}