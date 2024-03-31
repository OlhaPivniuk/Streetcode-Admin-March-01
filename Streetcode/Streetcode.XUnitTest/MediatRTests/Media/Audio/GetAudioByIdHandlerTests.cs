namespace Streetcode.XUnitTest.MediatRTests.Media.Audio;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Dto.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetAudioByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IBlobService> _mockBlobService;

    public GetAudioByIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockBlobService = new Mock<IBlobService>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsOkResult_WhenIdExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsAudio(id);
        MockMapperSetup(id);

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryCallGetFirstOrDefaultAsyncOnlyOnce_WhenAudioExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsAudio(id);
        MockMapperSetup(id);

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.AudioRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Audio, bool>>>(),
               It.IsAny<Func<IQueryable<Audio>,
               IIncludableQueryable<Audio, object>>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_MapperCallMapOnlyOnce_WhenAudioExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsAudio(id);
        MockMapperSetup(id);

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<AudioDto>(It.IsAny<Audio>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsVideoWithCorrectId_WhenAudioExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsAudio(id);
        MockMapperSetup(id);

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Value.Id);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnAudioDto_WhenAudioExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsAudio(id);
        MockMapperSetup(id);

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.IsType<AudioDto>(result.Value);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnFail_WhenAudioIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_WhenAudioIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAudioByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        var expectedMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(DAL.Entities.Media.Audio),
                id);

        // Act
        var result = await handler.Handle(new GetAudioByIdQuery(id), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<AudioDto>(It.IsAny<Audio>()))
            .Returns(new AudioDto { Id = id });
    }

    private void MockRepositorySetupReturnsAudio(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.AudioRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Audio, bool>>>(),
               It.IsAny<Func<IQueryable<Audio>,
               IIncludableQueryable<Audio, object>>>()))
            .ReturnsAsync(new Audio { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.AudioRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Audio, bool>>>(),
                It.IsAny<Func<IQueryable<Audio>,
            IIncludableQueryable<Audio, object>>>()))
            .ReturnsAsync((IEnumerable<Audio>?)null);
    }
}
