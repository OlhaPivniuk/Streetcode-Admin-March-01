using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.DAL.Entities.Timeline;
using Xunit;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using FluentAssertions;

using HistoricalContextEntity = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem;

public class CreateTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    public CreateTimelineItemHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WithValidInputAndWhenHistoricalContextIsNull()
    {
        // Arrange
        var request = new CreateTimelineItemRequestDto()
        {
            Date = DateTime.Now,
            DateViewPattern = 0,
            Title = "Valid",
            StreetcodeId = 1,
            Description = "Description",
        };
        MockRepositoryWrapperSetup(1, 1, string.Empty);
        MockMapperSetup();
        var command = new CreateTimelineItemCommand(request);
        var handler = new CreateTimelineItemHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WithValidInputAndWhenHistoricalContextIsNotNull()
    {
        // Arrange
        var request = new CreateTimelineItemRequestDto()
        {
            Date = DateTime.Now,
            DateViewPattern = 0,
            Title = "Valid",
            StreetcodeId = 1,
            HistoricalContext = GetHistoricalContextDto(),
            Description = "Description",
        };
        MockRepositoryWrapperSetup(1, 1, "Valid");
        MockMapperSetup();
        var command = new CreateTimelineItemCommand(request);
        var handler = new CreateTimelineItemHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenStreetcodeIdNotFound()
    {
        // Arrange
        var request = new CreateTimelineItemRequestDto()
        {
            Date = DateTime.Now,
            DateViewPattern = 0,
            Title = "Valid",
            StreetcodeId = 0,
            HistoricalContext = GetHistoricalContextDto(),
            Description = "Description",
        };
        MockRepositoryWrapperSetup(0, 1, "Valid");
        MockMapperSetup();
        var command = new CreateTimelineItemCommand(request);
        var handler = new CreateTimelineItemHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenSaveChangesAsyncWorkedNotCorrect()
    {
        // Arrange
        var request = new CreateTimelineItemRequestDto()
        {
            Date = DateTime.Now,
            DateViewPattern = 0,
            Title = "Valid",
            StreetcodeId = 1,
            HistoricalContext = GetHistoricalContextDto(),
            Description = "Description",
        };
        MockRepositoryWrapperSetup(1, -1, "Valid");
        MockMapperSetup();
        var command = new CreateTimelineItemCommand(request);
        var handler = new CreateTimelineItemHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    private void MockRepositoryWrapperSetup(int streetcodeId, int saveChangesOption, string historicalContextTitle)
    {
        var streetcode = streetcodeId switch
        {
            1 => new StreetcodeContent { Id = streetcodeId },
            _ => null
        };

        var historicalContext = !string.IsNullOrEmpty(historicalContextTitle) switch
        {
            true => new HistoricalContextEntity { Title = historicalContextTitle },
            _ => null
        };

        _mockRepositoryWrapper.Setup(x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>,
                IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper.Setup(x => x.TimelineRepository.Create(GetTimelineItem()));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(saveChangesOption);

        _mockRepositoryWrapper.Setup(x => x.HistoricalContextRepository.Create(GetHistoricalContext()));

        _mockRepositoryWrapper.Setup(x => x.HistoricalContextRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistoricalContextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<HistoricalContextEntity>,
                IIncludableQueryable<HistoricalContextEntity, object>>>()))
            .ReturnsAsync(historicalContext);

        _mockRepositoryWrapper.Setup(x => x.TimelineRepository.Create(GetTimelineItem()));

        _mockRepositoryWrapper.Setup(x => x.HistoricalContextTimelineRepository.Create(GetHistoricalContextTimeline()));

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
            .Returns(new System.Transactions.TransactionScope());
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x.Map<TimelineItemEntity>(It.IsAny<CreateTimelineItemRequestDto>()))
            .Returns(GetTimelineItem());

        _mockMapper.Setup(x => x.Map<TimelineItemDto>(It.IsAny<TimelineItemEntity>()))
            .Returns(GetTimelineItemDto());

        _mockMapper.Setup(x => x.Map<HistoricalContextEntity>(It.IsAny<HistoricalContextDto>()))
            .Returns(GetHistoricalContext());

        _mockMapper.Setup(x => x.Map<HistoricalContextDto>(It.IsAny<HistoricalContextEntity>()))
            .Returns(GetHistoricalContextDto());
    }

    private static TimelineItemEntity GetTimelineItem()
    {
        return new TimelineItemEntity { Id = 1, StreetcodeId = 1, Title = "Valid" };
    }

    private static TimelineItemDto GetTimelineItemDto()
    {
        return new TimelineItemDto { Id = 1, Title = "Valid" };
    }

    private static HistoricalContextEntity GetHistoricalContext()
    {
        return new HistoricalContextEntity { Id = 1, Title = "Valid" };
    }

    private static HistoricalContextDto GetHistoricalContextDto()
    {
        return new HistoricalContextDto { Id = 1, Title = "Valid" };
    }

    private static HistoricalContextTimeline GetHistoricalContextTimeline()
    {
        return new HistoricalContextTimeline { TimelineId = 1, HistoricalContextId = 1 };
    }
}
