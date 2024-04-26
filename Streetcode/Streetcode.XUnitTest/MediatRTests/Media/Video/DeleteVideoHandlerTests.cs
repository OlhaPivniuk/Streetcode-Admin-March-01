using System.Linq.Expressions;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.Delete;
using Streetcode.BLL.MediatR.Video.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video
{
    public class DeleteVideoHandlerTests
    {
        private const int SUCCESSFULSAVE = 1;
        private const int FAILEDSAVE = -1;

        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;

        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public DeleteVideoHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
        {
            // Arrange
            var request = GetValidVideoRecordRequest();
            var video = new VideoEntity();
            SetupMock(SUCCESSFULSAVE, video);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
        {
            // Arrange
            var request = GetValidVideoRecordRequest();
            var expectedType = typeof(Result<DeleteVideoResponseDto>);
            var video = new VideoEntity();
            SetupMock(SUCCESSFULSAVE, video);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.Should().BeOfType(expectedType);
        }

        [Fact]
        public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
        {
            // Arrange
            var request = GetValidVideoRecordRequest();
            var video = new VideoEntity();
            SetupMock(FAILEDSAVE, video);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnErrorDeleteFailed_IfSavingOperationFailed()
        {
            // Arrange
            var request = GetValidVideoRecordRequest();
            var video = new VideoEntity();
            SetupMock(FAILEDSAVE, video);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            var expectedErrorMessage = string.Format(
            ErrorMessages.DeleteFailed,
            typeof(VideoEntity).Name,
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
            var request = GetValidVideoRecordRequest();
            var video = new VideoEntity();
            SetupMock(SUCCESSFULSAVE, video);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            await handler.Handle(command, _cancellationToken);

            // Assert
            _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
        }

        [Fact]
        public async Task Handle_ShouldLogError_IfVideoNotFound()
        {
            // Arrange
            var request = GetInvalidVideoRecordRequest();
            SetupMock(SUCCESSFULSAVE, video: null);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            await handler.Handle(command, _cancellationToken);

            // Assert
            _mockLogger.Verify(x => x.LogError(request, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnErrorIfVideoNotFound()
        {
            // Arrange
            var request = GetInvalidVideoRecordRequest();
            SetupMock(SUCCESSFULSAVE, video: null);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
        }

        [Fact]
        public async Task Handle_ShouldNotDeleteIfVideoNotFound()
        {
            // Arrange
            var request = GetInvalidVideoRecordRequest();
            SetupMock(SUCCESSFULSAVE, video: null);
            var handler = DeleteHandler();
            var command = new DeleteVideoCommand(request);

            // Act
            await handler.Handle(command, _cancellationToken);

            // Assert
            _mockRepositoryWrapper.Verify(x => x.VideoRepository.Delete(It.IsAny<VideoEntity>()), Times.Never);
        }

        private DeleteVideoHandler DeleteHandler()
        {
            return new DeleteVideoHandler(
                _mockRepositoryWrapper.Object,
                _mockLogger.Object);
        }

        private void SetupMock(int saveChangesAsyncResult, VideoEntity? video)
        {
            _mockRepositoryWrapper
                .Setup(repo => repo.VideoRepository.GetFirstOrDefaultAsync(
                    AnyEntityPredicate<VideoEntity>(),
                    AnyEntityInclude<VideoEntity>()))
                .ReturnsAsync(video);

            _mockRepositoryWrapper
                .Setup(repo => repo.VideoRepository.Delete(It.IsAny<VideoEntity>()));

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

        private static DeleteVideoRequestDto GetValidVideoRecordRequest()
        {
            return new(1);
        }

        private static DeleteVideoRequestDto GetInvalidVideoRecordRequest()
        {
            return new(-1);
        }
    }
}
