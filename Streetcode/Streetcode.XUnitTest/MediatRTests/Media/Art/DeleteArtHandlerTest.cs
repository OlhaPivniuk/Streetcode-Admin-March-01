using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.DTO.Media.Art;
using Xunit;
using FluentAssertions;
using FluentResults;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.MediatRTests.Media.Art;

public class DeleteArtHandlerTest
{
    private const int SUCCESSFULRESULT = 1;
    private const int FAILURERESULT = -1;

    private const int EXISTARTID = 1;
    private const int NONEXISTARTID = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    public DeleteArtHandlerTest()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenSaveChangesAsyncWorkedCorrectAndIdIsValid()
    {
        // Arrange
        var request = new DeleteArtRequestDto(EXISTARTID);
        SetupMockRepositoryWrapper(request.Id, SUCCESSFULRESULT);
        var command = new DeleteArtCommand(request);
        var handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenArtDoesNotExist()
    {
        // Arrange
        var request = new DeleteArtRequestDto(NONEXISTARTID);
        SetupMockRepositoryWrapper(request.Id, SUCCESSFULRESULT);
        SetupMockLogger(request.Id, ArtNotFoundErrorMessage(NONEXISTARTID));
        var command = new DeleteArtCommand(request);
        var handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenSaveChangesAsyncWorkedNotCorrect()
    {
        // Arrange
        var request = new DeleteArtRequestDto(EXISTARTID);
        SetupMockRepositoryWrapper(request.Id, FAILURERESULT);
        SetupMockLogger(request.Id, DeleteFailedErrorMessage(request.Id));
        var command = new DeleteArtCommand(request);
        var handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnExpectedErrorMessage_WhenArtNotFound()
    {
        // Arrange
        var request = new DeleteArtRequestDto(NONEXISTARTID);
        SetupMockRepositoryWrapper(request.Id, SUCCESSFULRESULT);
        SetupMockLogger(request.Id, ArtNotFoundErrorMessage(request.Id));
        var command = new DeleteArtCommand(request);
        var handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == ArtNotFoundErrorMessage(request.Id));
    }

    [Fact]
    public async Task Handle_Should_ReturnExpectedErrorMessage_DeleteFailed()
    {
        // Arrange
        var request = new DeleteArtRequestDto(EXISTARTID);
        SetupMockRepositoryWrapper(request.Id, FAILURERESULT);
        SetupMockLogger(request.Id, DeleteFailedErrorMessage(request.Id));
        var command = new DeleteArtCommand(request);
        var handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == DeleteFailedErrorMessage(request.Id));
    }

    [Fact]
    public async Task Handle_Result_Should_BeOfExpectedType()
    {
        // Arrange
        var request = new DeleteArtRequestDto(EXISTARTID);
        SetupMockRepositoryWrapper(request.Id, SUCCESSFULRESULT);
        var command = new DeleteArtCommand(request);
        var handler = new DeleteArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType(typeof(Result<DeleteArtResponseDto>));
    }

    private void SetupMockRepositoryWrapper(int id, int result)
    {
        var art = id switch
        {
            EXISTARTID => new ArtEntity { Id = id },
            _ => null
        };

        _mockRepositoryWrapper.Setup(x => x.ArtRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<ArtEntity, bool>>>(),
            It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync(art);

        _mockRepositoryWrapper.Setup(x => x.ArtRepository.Delete(It.IsAny<ArtEntity>()));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(result);
    }

    private void SetupMockLogger(int id, string errorMessage)
    {
        _mockLogger.Setup(x => x.LogError(id, errorMessage));
    }

    private static string ArtNotFoundErrorMessage(int id)
    {
        return string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(ArtEntity).Name,
            id);
    }

    private static string DeleteFailedErrorMessage(int id)
    {
        return string.Format(
            ErrorMessages.DeleteFailed,
            typeof(ArtEntity).Name,
            id);
    }
}
