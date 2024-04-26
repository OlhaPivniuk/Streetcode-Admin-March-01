using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StatisticRecordEntity = Streetcode.DAL.Entities.Analytics.StatisticRecord;

namespace Streetcode.XUnitTest.MediatRTests.Analytics.StatisticRecord;

public class CreateStatisticRecordHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;
    private const int MINADDRESSLENGTH = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateStatisticRecordHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateStatisticRecordRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeId()
    {
        // Arrange
        var request = new CreateStatisticRecordRequestDto(
            StreetcodeId: int.MaxValue,
            StreetcodeCoordinateId: 1,
            Address: new string('a', MINADDRESSLENGTH));

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeCoordinateId()
    {
        // Arrange
        var request = new CreateStatisticRecordRequestDto(
            StreetcodeId: 1,
            StreetcodeCoordinateId: int.MaxValue,
            Address: new string('a', MINADDRESSLENGTH));

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeCoordinate).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStatisticRecordRequest();
        var expectedType = typeof(Result<CreateStatisticRecordResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateStatisticRecordRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreateStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStatisticRecordRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateStatisticRecordCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private CreateStatisticRecordHandler CreateHandler()
    {
        return new CreateStatisticRecordHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateStatisticRecordRequestDto request, int saveChangesAsyncResult)
    {
        var streetcode = request.StreetcodeId switch
        {
            1 => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        var streetcodeCoordinate = request.StreetcodeCoordinateId switch
        {
            1 => new StreetcodeCoordinate { Id = request.StreetcodeCoordinateId },
            _ => null,
        };

        var statisticRecord = new StatisticRecordEntity { Id = 1, StreetcodeId = 1, Address = new string('a', MINADDRESSLENGTH) };

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeContent>(),
                AnyEntityInclude<StreetcodeContent>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeCoordinate>(),
                AnyEntityInclude<StreetcodeCoordinate>()))
            .ReturnsAsync(streetcodeCoordinate);

        _mockRepositoryWrapper.Setup(repo => repo.StatisticRecordRepository.Create(
            It.IsAny<StatisticRecordEntity>())).Returns(statisticRecord);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<StatisticRecordEntity>(It.IsAny<CreateStatisticRecordRequestDto>())).Returns(statisticRecord);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreateStatisticRecordRequestDto GetValidCreateStatisticRecordRequest()
    {
        return new(
            StreetcodeId: 1,
            StreetcodeCoordinateId: 1,
            Address: new string('a', MINADDRESSLENGTH));
    }
}
