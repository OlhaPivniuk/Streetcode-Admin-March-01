namespace Streetcode.XUnitTest.MediatRTests.Media.Video;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Dto.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetById;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetVideoByIdTest
{
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;

    public GetVideoByIdTest()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsOkResult_WhenIdExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsVideo(id);
        MockMapperSetup(id);

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryCallGetFirstOrDefaultAsyncOnlyOnce_WhenVideoExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsVideo(id);
        MockMapperSetup(id);

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.VideoRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Video, bool>>>(),
               It.IsAny<Func<IQueryable<Video>,
               IIncludableQueryable<Video, object>>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_MapperCallMapOnlyOnce_WhenVideoExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsVideo(id);
        MockMapperSetup(id);

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<VideoDto>(It.IsAny<Video>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsVideoWithCorrectId_WhenVideoExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsVideo(id);
        MockMapperSetup(id);

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Value.Id);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnVideoDto_WhenVideoExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsVideo(id);
        MockMapperSetup(id);

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.IsType<VideoDto>(result.Value);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnFail_WhenVideoIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_WhenVideoIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetVideoByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedMessage = string.Format(
               ErrorMessages.EntityByIdNotFound,
               nameof(DAL.Entities.Media.Video),
               id);

        // Act
        var result = await handler.Handle(new GetVideoByIdQuery(id), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<VideoDto>(It.IsAny<Video>()))
            .Returns(new VideoDto { Id = id });
    }

    private void MockRepositorySetupReturnsVideo(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.VideoRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Video, bool>>>(),
               It.IsAny<Func<IQueryable<Video>,
               IIncludableQueryable<Video, object>>>()))
            .ReturnsAsync(new Video { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.VideoRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Video, bool>>>(),
                It.IsAny<Func<IQueryable<Video>,
            IIncludableQueryable<Video, object>>>()))
            .ReturnsAsync((IEnumerable<Video>?)null);
    }
}
