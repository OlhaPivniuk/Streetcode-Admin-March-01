using System.Linq.Expressions;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.StreetcodeToponym;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.StreetcodeToponym.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Toponyms;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using StreetcodeToponymEntity = Streetcode.DAL.Entities.Toponyms.StreetcodeToponym;

namespace Streetcode.XUnitTest.MediatRTests.StreetcodeToponym;

public class DeleteStreetcodeToponymHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteStreetcodeToponymHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        var expectedType = typeof(Result<DeleteStreetcodeToponymResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeToponymCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorDeleteFailed_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeToponymCommand(request);

        var expectedErrorMessage = string.Format(
        ErrorMessages.DeleteFailed,
        typeof(StreetcodeToponymEntity).Name,
        GetPhysicalStreetCode(request.StreetcodeId, request.ToponymId));

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
        var request = GetValidCreateStreetcodeToponymRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeleteStreetcodeToponymCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private DeleteStreetcodeToponymHandler DeleteHandler()
    {
        return new DeleteStreetcodeToponymHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(DeleteStreetcodeToponymRequestDto request, int saveChangesAsyncResult)
    {
        var streetcode = request.StreetcodeId switch
        {
            1 => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        var toponym = request.ToponymId switch
        {
            1 => new Toponym { Id = request.ToponymId },
            _ => null,
        };

        var streetcodeToponym = new StreetcodeToponymEntity { StreetcodeId = 1, ToponymId = 1 };

        _mockRepositoryWrapper
                .Setup(repo => repo.StreetcodeToponymRepository.GetFirstOrDefaultAsync(
                    AnyEntityPredicate<StreetcodeToponymEntity>(),
                    AnyEntityInclude<StreetcodeToponymEntity>()))
                .ReturnsAsync(streetcodeToponym);

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeToponymRepository.Delete(streetcodeToponym));

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

    private static DeleteStreetcodeToponymRequestDto GetValidCreateStreetcodeToponymRequest()
    {
        return new(
            StreetcodeId: 1,
            ToponymId: 1);
    }

    private static string GetPhysicalStreetCode(int streetcodeId, int toponymId)
    {
        string template = "000000";
        string phisicalStreetcode = template.Remove(template.Length - streetcodeId.ToString().Length)
            + streetcodeId.ToString();
        phisicalStreetcode += template.Remove(template.Length - toponymId.ToString().Length)
            + toponymId.ToString();
        return phisicalStreetcode;
    }
}
