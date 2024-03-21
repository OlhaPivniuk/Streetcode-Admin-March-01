﻿namespace Streetcode.XUnitTest.MediatRTests.Media.Audio;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetAll;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetAllAudioHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IBlobService> _mockBlobService;

    public GetAllAudioHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockBlobService = new Mock<IBlobService>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WithListOfAudios_WhenAudiosFound()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsFailedResult_WhenAudiosNotFound()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        MockMapperSetup();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_LogsError_WhenAudiosNotFound()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        MockMapperSetup();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            logger => logger.LogError(
                It.IsAny<GetAllAudiosQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsCollectionOfCorrectCount_WhenAudiosFound()
    {
        // Arrange
        var mockVideo = GetAudioList();
        var expectedCount = mockVideo.Count;

        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async Task Handle_MapperShouldMapOnlyOnce_WhenAudiosFound()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<AudioDto>>(It.IsAny<IEnumerable<Audio>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCollectionOfAudioDto_WhenAudiosFound()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        Assert.IsType<List<AudioDto>>(result.Value);
    }

    [Fact]
    public async Task Handle_ReturnsFailedResult_WhenAudiosAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogCorrectErrorMessage_WhenAudiosAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllAudiosHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        var expectedError = "Cannot find any audios";

        // Act
        var result = await handler.Handle(new GetAllAudiosQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedError, result.Errors.First().Message);
    }

    private static List<Streetcode.DAL.Entities.Media.Audio> GetAudioList()
    {
        return new List<Streetcode.DAL.Entities.Media.Audio>
        {
            new ()
            {
                Id = 1,
                Title = "Audio Title 1",
                BlobName = "audio1.mp3",
                MimeType = "audio/mpeg",
                Base64 = "base64_encoded_audio_data_1",
            },
            new ()
            {
                Id = 2,
                Title = "Audio Title 2",
                BlobName = "audio2.mp3",
                MimeType = "audio/mpeg",
                Base64 = "base64_encoded_audio_data_2",
            },
            new ()
            {
                Id = 3,
                Title = "Audio Title 3",
                BlobName = "audio3.mp3",
                MimeType = "audio/mpeg",
                Base64 = "base64_encoded_audio_data_3",
            },
        };
    }

    private static List<AudioDto> GetAudioDtoList()
    {
        return new List<AudioDto>
        {
            new ()
            {
                Id = 1,
                Description = "Description 1",
                BlobName = "audio1.mp3",
                Base64 = "base64_encoded_data_1",
                MimeType = "audio/mpeg",
            },
            new ()
            {
                Id = 2,
                Description = "Description 2",
                BlobName = "audio2.mp3",
                Base64 = "base64_encoded_data_2",
                MimeType = "audio/mpeg"
            },
            new ()
            {
                Id = 3,
                Description = "Description 3",
                BlobName = "audio3.mp3",
                Base64 = "base64_encoded_data_3",
                MimeType = "audio/mpeg",
            },
        };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<AudioDto>>(It.IsAny<IEnumerable<Streetcode.DAL.Entities.Media.Audio>>()))
            .Returns(GetAudioDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.AudioRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Audio, bool>>>(),
                It.IsAny<Func<IQueryable<Audio>,
            IIncludableQueryable<Audio, object>>>()))
            .ReturnsAsync(GetAudioList());
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