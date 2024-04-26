using System.Linq.Expressions;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeCoordinateEntity = Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.MediatRTests.AdditionalContent.Coordinates.Types;

public class DeleteStreetcodeCoordinateHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteStreetcodeCoordinateHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidDeleteStreetcodeCoordinateRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeleteStreetcodeCoordinateRequest();
        var expectedType = typeof(Result<DeleteStreetcodeCoordinateResponseDto>);
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteStreetcodeCoordinateRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeCoordinateCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorDeleteFailed_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeleteStreetcodeCoordinateRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeCoordinateCommand(request);

        var expectedErrorMessage = string.Format(
        ErrorMessages.DeleteFailed,
        typeof(StreetcodeCoordinateEntity).Name,
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
        var request = GetValidDeleteStreetcodeCoordinateRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeCoordinateCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private DeleteStreetcodeCoordinateHandler DeleteHandler()
    {
        return new DeleteStreetcodeCoordinateHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(int saveChangesAsyncResult)
    {
        var streetcodeCoordinate = new StreetcodeCoordinateEntity { Id = 1, StreetcodeId = 1, Longtitude = 1, Latitude = 1 };

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeCoordinateEntity>(),
                AnyEntityInclude<StreetcodeCoordinateEntity>()))
            .ReturnsAsync(streetcodeCoordinate);

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeCoordinateRepository.Delete(streetcodeCoordinate));

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

    private static DeleteStreetcodeCoordinateRequestDto GetValidDeleteStreetcodeCoordinateRequest()
    {
        return new(Id: 1);
    }
}
