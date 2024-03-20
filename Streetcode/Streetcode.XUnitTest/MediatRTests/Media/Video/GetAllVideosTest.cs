﻿using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetAll;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video
{
    public class GetAllVideosHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public GetAllVideosHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ReturnsOkResult_WithListOfVideos_WhenVideosFound()
        {
            // Arrange
            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ReturnsFailedResult_WhenVideosNotFound()
        {
            // Arrange
            MockRepositorySetupReturnsNull();
            MockMapperSetup();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_LogsError_WhenVideosNotFound()
        {
            // Arrange
            MockRepositorySetupReturnsNull();
            MockMapperSetup();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                logger => logger.LogError(
                    It.IsAny<GetAllVideosQuery>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsCollectionOfCorrectCount_WhenVideosFound()
        {
            // Arrange
            var mockVideo = GetVideoList();
            var expectedCount = mockVideo.Count;

            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);
            var actualCount = result.Value.Count();

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task Handle_MapperShouldMapOnlyOnce_WhenVideosFound()
        {
            // Arrange
            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            _mockMapper.Verify(
                mapper =>
                mapper.Map<IEnumerable<VideoDTO>>(It.IsAny<IEnumerable< Streetcode.DAL.Entities.Media.Video>> ()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnCollectionOfFactDto_WhenVideosFound()
        {
            // Arrange
            MockRepositorySetupReturnsData();
            MockMapperSetup();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            Assert.IsType<List<VideoDTO>>(result.Value);
        }

        [Fact]
        public async Task Handle_ReturnsFailedResult_WhenVideosAreNull()
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Fact]
        public async Task Handle_ShouldLogCorrectErrorMessage_WhenVideosAreNull()
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetAllVideosHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);

            var expectedError = "Cannot find any videos";

            // Act
            var result = await handler.Handle(new GetAllVideosQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(expectedError, result.Errors.First().Message);
        }

        private static List<Streetcode.DAL.Entities.Media.Video> GetVideoList()
        {
            return new List<Streetcode.DAL.Entities.Media.Video>
            {
                new ()
                {
                    Id = 1,
                    Title = "Video Title 1",
                    Description = "Video Description 1",
                    Url = "https://example.com/video1",
                    StreetcodeId = 1,
                },
                new ()
                {
                    Id = 2,
                    Title = "Video Title 2",
                    Description = "Video Description 2",
                    Url = "https://example.com/video2",
                    StreetcodeId = 2,
                },
                new ()
                {
                    Id = 3,
                    Title = "Video Title 3",
                    Description = "Video Description 3",
                    Url = "https://example.com/video3",
                    StreetcodeId = 3,
                },
            };
        }

        private static List<VideoDTO> GetVideoDtoList()
        {
            return new List<VideoDTO>
            {
                new ()
                {
                   Id = 1,
                   Description = "Video Description 1",
                   Url = "https://example.com/video1",
                },
                new ()
                {
                   Id = 2,
                   Description = "Video Description 2",
                   Url = "https://example.com/video2",
                },
                new ()
                {
                   Id = 3,
                   Description = "Video Description 3",
                   Url = "https://example.com/video3",
                },
            };
        }

        private void MockMapperSetup()
        {
            _mockMapper.Setup(x => x
                .Map<IEnumerable<VideoDTO>>(It.IsAny<IEnumerable<Streetcode.DAL.Entities.Media.Video>>()))
                .Returns(GetVideoDtoList());
        }

        private void MockRepositorySetupReturnsData()
        {
            _mockRepositoryWrapper.Setup(x => x.VideoRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<Streetcode.DAL.Entities.Media.Video, bool>>>(),
                    It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.Media.Video>,
                IIncludableQueryable<Streetcode.DAL.Entities.Media.Video, object>>>()))
                .ReturnsAsync(GetVideoList());
        }

        private void MockRepositorySetupReturnsNull()
        {
            _mockRepositoryWrapper.Setup(x => x.VideoRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<Streetcode.DAL.Entities.Media.Video, bool>>>(),
                    It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.Media.Video>,
                IIncludableQueryable<Streetcode.DAL.Entities.Media.Video, object>>>()))
                .ReturnsAsync((IEnumerable<Streetcode.DAL.Entities.Media.Video>?)null);
        }
    }
}