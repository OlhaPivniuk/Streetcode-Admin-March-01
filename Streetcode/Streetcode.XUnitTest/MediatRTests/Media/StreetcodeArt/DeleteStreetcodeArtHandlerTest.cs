using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Media.StreetcodeArt.Delete;
using Streetcode.BLL.DTO.Media.Art;
using Xunit;
using FluentAssertions;
using FluentResults;

using StreetcodeArtEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeArt;

namespace Streetcode.XUnitTest.MediatRTests.Media.StreetcodeArt;

public class DeleteStreetcodeArtHandlerTest
{
    private const int SUCCESSFULRESULT = 1;
    private const int FAILURERESULT = -1;

    private const int EXISTID = 1;
    private const int NONEXISTID = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    public DeleteStreetcodeArtHandlerTest()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenSaveChangesAsyncWorkedCorrectAndIdIsValid()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(EXISTID, EXISTID);
        SetupMockRepositoryWrapper(request, SUCCESSFULRESULT);
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenStreetcodeIdDoesNotExist()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(EXISTID, NONEXISTID);
        SetupMockRepositoryWrapper(request, SUCCESSFULRESULT);
        SetupMockLogger(request, StreetcodeArtNotFoundErrorMessage());
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenArtIdDoesNotExist()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(NONEXISTID, EXISTID);
        SetupMockRepositoryWrapper(request, SUCCESSFULRESULT);
        SetupMockLogger(request, StreetcodeArtNotFoundErrorMessage());
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Result_Should_BeOfExpectedType()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(EXISTID, EXISTID);
        SetupMockRepositoryWrapper(request, SUCCESSFULRESULT);
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType(typeof(Result<DeleteStreetcodeArtResponeDto>));
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenSaveChangesAsyncWorkedNotCorrect()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(EXISTID, EXISTID);
        SetupMockRepositoryWrapper(request, FAILURERESULT);
        SetupMockLogger(request, DeleteFailedErrorMessage());
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnExpectedErrorMessage_WhenArtNotFound()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(EXISTID, NONEXISTID);
        SetupMockRepositoryWrapper(request, SUCCESSFULRESULT);
        SetupMockLogger(request, StreetcodeArtNotFoundErrorMessage());
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == StreetcodeArtNotFoundErrorMessage());
    }

    [Fact]
    public async Task Handle_Should_ReturnExpectedErrorMessage_WhenDeleteFailed()
    {
        // Arrange
        var request = new DeleteStreetcodeArtRequestDto(EXISTID, EXISTID);
        SetupMockRepositoryWrapper(request, FAILURERESULT);
        SetupMockLogger(request, DeleteFailedErrorMessage());
        var command = new DeleteStreetcodeArtCommand(request);
        var handler = new DeleteStreetcodeArtHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == DeleteFailedErrorMessage());
    }

    private void SetupMockRepositoryWrapper(DeleteStreetcodeArtRequestDto request, int result)
    {
        int streetcodeId = request.StreetcodeId switch
        {
            EXISTID => 1,
            _ => 0
        };

        int artId = request.ArtId switch
        {
            EXISTID => 1,
            _ => 0
        };

        StreetcodeArtEntity streetcodeArt = new StreetcodeArtEntity { ArtId = artId, StreetcodeId = streetcodeId };

        _mockRepositoryWrapper.Setup(x => x.StreetcodeArtRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeArtEntity, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeArtEntity>, IIncludableQueryable<StreetcodeArtEntity, object>>>()))
            .ReturnsAsync(StreetcodeArt(streetcodeArt));

        _mockRepositoryWrapper.Setup(x => x.StreetcodeArtRepository.Delete(It.IsAny<StreetcodeArtEntity>()));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(result);
    }

    private static StreetcodeArtEntity? StreetcodeArt(StreetcodeArtEntity streetcodeArt)
    {
        if (streetcodeArt.ArtId == 0 || streetcodeArt.StreetcodeId == 0)
        {
            return null;
        }

        return streetcodeArt;
    }

    private void SetupMockLogger(DeleteStreetcodeArtRequestDto request, string errorMessage)
    {
        _mockLogger.Setup(x => x.LogError(request, errorMessage));
    }

    private static string StreetcodeArtNotFoundErrorMessage()
    {
        return string.Format(
            ErrorMessages.EntityByPrimaryKeyNotFound,
            typeof(StreetcodeArtEntity).Name);
    }

    private static string DeleteFailedErrorMessage()
    {
        return string.Format(
            ErrorMessages.FailedToDeleteByPrimaryKey,
            typeof(StreetcodeArtEntity).Name);
    }
}
