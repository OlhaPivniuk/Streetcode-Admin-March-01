using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Sources;

public class DeleteCategoryHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public DeleteCategoryHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_DeletesCategoryAndReturnsOkResult_IfCategoryFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingCategoryId(id);

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryShouldCallGetFirstOrDefaultAsyncOnlyOnce_IfCategoryExists(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingCategoryId(id);

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.SourceCategoryRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
               It.IsAny<Func<IQueryable<SourceLinkCategory>,
               IIncludableQueryable<SourceLinkCategory, object>>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryShouldCallDeleteOnlyOnce_IfCategoryExists(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingCategoryId(id);

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.SourceCategoryRepository
                .Delete(It.IsAny<SourceLinkCategory>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsFailResult_IfCategoryNotFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithNotExistingCategoryId();

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_IfCategoryIsNotFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithNotExistingCategoryId();

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(SourceLinkCategory),
            id);

        // Act
        var result = await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsFailResult_IfSaveChangesAsyncNotSuccessful(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingCategoryId(id);
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_IfSaveChangesAsyncNotSuccessful(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingCategoryId(id);
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

        var handler = new DeleteCategoryHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            nameof(SourceLinkCategory),
            id);

        // Act
        var result = await handler.Handle(new DeleteCategoryCommand(id), CancellationToken.None);

        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    private static SourceLinkCategory GetCategory(int id)
    {
        return new SourceLinkCategory
        {
            Id = id,
        };
    }

    private static SourceLinkCategory? GetCategoryWithNotExistingId()
    {
        return null;
    }

    private void MockRepositoryWrapperSetupWithExistingCategoryId(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.SourceCategoryRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
                It.IsAny<Func<IQueryable<SourceLinkCategory>,
                IIncludableQueryable<SourceLinkCategory, object>>>()))
            .ReturnsAsync(GetCategory(id));

        _mockRepositoryWrapper.Setup(x => x.SourceCategoryRepository
            .Delete(GetCategory(id)));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
    }

    private void MockRepositoryWrapperSetupWithNotExistingCategoryId()
    {
        _mockRepositoryWrapper.Setup(x => x.SourceCategoryRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategory, bool>>>(),
                It.IsAny<Func<IQueryable<SourceLinkCategory>,
                IIncludableQueryable<SourceLinkCategory, object>>>()))
            .ReturnsAsync(GetCategoryWithNotExistingId());
    }
}
