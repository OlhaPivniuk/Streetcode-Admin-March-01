using System.Linq.Expressions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.Update;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;

using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video.Update
{
    public class UpdateVideoHandlerTests
    {
        private const int SUCCESSFULSAVE = 1;
        private const int FAILEDSAVE = -1;

        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public UpdateVideoHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
        {
            // Arrange
            var request = GetValidUpdateVideoRequest();
            SetupMock(request, SUCCESSFULSAVE);
            var handler = CreateHandler();
            var command = new UpdateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
        {
            // Arrange
            var request = GetValidUpdateVideoRequest();
            var expectedType = typeof(Result<UpdateVideoResponseDto>);
            SetupMock(request, SUCCESSFULSAVE);
            var handler = CreateHandler();
            var command = new UpdateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.Should().BeOfType(expectedType);
        }

        [Fact]
        public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
        {
            // Arrange
            var request = GetValidUpdateVideoRequest();
            SetupMock(request, FAILEDSAVE);
            var handler = CreateHandler();
            var command = new UpdateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
        {
            // Arrange
            var request = GetValidUpdateVideoRequest();
            SetupMock(request, SUCCESSFULSAVE);
            var handler = CreateHandler();
            var command = new UpdateVideoCommand(request);

            // Act
            await handler.Handle(command, _cancellationToken);

            // Assert
            _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
        }

        private UpdateVideoHandler CreateHandler()
        {
            return new UpdateVideoHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        private void SetupMock(UpdateVideoRequestDto request, int saveChangesAsyncResult)
        {
            var video = new VideoEntity
            {
                Id = request.Id,
                StreetcodeId = 1,
                Title = request.Title,
                Description = request.Description,
                Url = request.Url
            };

            _mockRepositoryWrapper.Setup(repo => repo.VideoRepository.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<System.Func<VideoEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<VideoEntity>, IIncludableQueryable<VideoEntity, object>>>()))
                .ReturnsAsync(video);

            _mockRepositoryWrapper.Setup(repo => repo.VideoRepository.Update(
                It.IsAny<VideoEntity>()));

            _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

            _mockMapper
                .Setup(m => m.Map<VideoEntity>(It.IsAny<UpdateVideoRequestDto>())).Returns(video);

            _mockMapper
                .Setup(m => m.Map<UpdateVideoResponseDto>(It.IsAny<VideoEntity>())).Returns((VideoEntity video) => new UpdateVideoResponseDto(
                    video.Id,
                    video.StreetcodeId,
                    video.Title,
                    video.Description,
                    video.Url!));
        }

        private static UpdateVideoRequestDto GetValidUpdateVideoRequest()
        {
            return new UpdateVideoRequestDto(1, "Updated Video Title", "Updated Video Description", "https://example.com/updated_video");
        }
    }
}