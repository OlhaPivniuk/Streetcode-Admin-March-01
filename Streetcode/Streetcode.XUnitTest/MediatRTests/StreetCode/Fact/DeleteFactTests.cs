namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact;

using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using Streetcode.BLL.Resources.Errors;

public class DeleteFactTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public DeleteFactTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_DeletesFactAndReturnsOkResult_IfFactFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryShouldCallGetFirstOrDefaultAsyncOnlyOnce_IfFactExists(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.FactRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Fact, bool>>>(),
               It.IsAny<Func<IQueryable<Fact>,
               IIncludableQueryable<Fact, object>>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_FactRepositoryShouldCallDeleteOnlyOnce_IfFactExists(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.FactRepository.Delete(It.IsAny<Fact>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsFailResult_IfFactNotFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithNotExistingFactId();

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_IfFactIsNotFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithNotExistingFactId();

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
        var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            nameof(Fact),
            id);

        // Act
        var result = await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsFailResult_IfSaveChangesAsyncNotSuccessful(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_IfSaveChangesAsyncNotSuccessful(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);
        var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            nameof(Fact),
            id);

        // Act
        var result = await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    private static Fact GetFact(int id)
    {
        return new Fact
        {
            Id = id,
        };
    }

    private static Fact? GetFactWithNotExistingId()
    {
        return null;
    }

    private void MockRepositoryWrapperSetupWithExistingFactId(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>,
                IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(GetFact(id));

        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .Delete(GetFact(id)));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
    }

    private void MockRepositoryWrapperSetupWithNotExistingFactId()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>,
                IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(GetFactWithNotExistingId());
    }
}
