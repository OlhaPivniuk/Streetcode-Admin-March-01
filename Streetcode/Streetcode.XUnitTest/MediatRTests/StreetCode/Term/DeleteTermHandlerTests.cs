using System.Linq.Expressions;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using TermEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Term;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Term;

public class DeleteTermHandlerTests
{
    private const int MINID = 1;
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteTermHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidTextRecordRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidTextRecordRequest();
        var expectedType = typeof(Result<DeleteTermResponseDto>);
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidTextRecordRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteTermCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorDeleteFailed_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidTextRecordRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteTermCommand(request);

        var expectedErrorMessage = string.Format(
        ErrorMessages.DeleteFailed,
        typeof(TermEntity).Name,
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
        var request = GetValidTextRecordRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteTermCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private DeleteTermHandler DeleteHandler()
    {
        return new DeleteTermHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(int saveChangesAsyncResult)
    {
        var term = new TermEntity { Id = MINID };

        _mockRepositoryWrapper
            .Setup(repo => repo.TermRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<TermEntity>(),
                AnyEntityInclude<TermEntity>()))
            .ReturnsAsync(term);

        _mockRepositoryWrapper
            .Setup(repo => repo.TermRepository.Delete(term));

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

    private static DeleteTermRequestDto GetValidTextRecordRequest()
    {
        return new(Id: MINID);
    }
}