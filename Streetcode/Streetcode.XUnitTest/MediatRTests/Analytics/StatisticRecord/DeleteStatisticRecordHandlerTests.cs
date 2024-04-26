using System.Linq.Expressions;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StatisticRecordEntity = Streetcode.DAL.Entities.Analytics.StatisticRecord;

namespace Streetcode.XUnitTest.MediatRTests.Analytics.StatisticRecord;

public class DeleteStatisticRecordHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteStatisticRecordHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidDeleteStatisticRecordRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeleteStatisticRecordRequest();
        var expectedType = typeof(Result<DeleteStatisticRecordResponseDto>);
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteStatisticRecordRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStatisticRecordCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorDeleteFailed_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteStatisticRecordRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStatisticRecordCommand(request);

        var expectedErrorMessage = string.Format(
        ErrorMessages.DeleteFailed,
        typeof(StatisticRecordEntity).Name,
        request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);
        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeleteStatisticRecordRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStatisticRecordCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private DeleteStatisticRecordHandler DeleteHandler()
    {
        return new DeleteStatisticRecordHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(int saveChangesAsyncResult)
    {
        var statisticRecord = new StatisticRecordEntity { Id = 1, StreetcodeId = 1, StreetcodeCoordinateId = 1 };

        _mockRepositoryWrapper
            .Setup(repo => repo.StatisticRecordRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StatisticRecordEntity>(),
                AnyEntityInclude<StatisticRecordEntity>()))
            .ReturnsAsync(statisticRecord);

        _mockRepositoryWrapper
            .Setup(repo => repo.StatisticRecordRepository.Delete(statisticRecord));

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static DeleteStatisticRecordRequestDto GetValidDeleteStatisticRecordRequest()
    {
        return new(Id: 1);
    }
}
