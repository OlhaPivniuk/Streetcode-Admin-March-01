using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using TimelineEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem
{
    public class DeleteTimelineItemHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;

        public DeleteTimelineItemHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_DeletesTimelineItemAndReturnsOkResult_IfTimelineItemFound(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(id);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_RepositoryShouldCallGetFirstOrDefaultAsyncOnlyOnce_IfTimelineItemExists(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(id);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);

            // Assert
            _mockRepositoryWrapper.Verify(
                repo =>
                repo.TimelineRepository.GetFirstOrDefaultAsync(
                   It.IsAny<Expression<Func<TimelineEntity, bool>>>(),
                   It.IsAny<Func<IQueryable<TimelineEntity>,
                   IIncludableQueryable<TimelineEntity, object>>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_RepositoryShouldCallDeleteOnlyOnce_IfTimelineItemExists(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(id);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);

            // Assert
            _mockRepositoryWrapper.Verify(
                repo =>
                repo.TimelineRepository
                    .Delete(It.IsAny<TimelineEntity>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ReturnsFailResult_IfTimelineItemNotFound(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithNotExistingTimelineItemyId();

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ShouldLogCorrectErrorMessage_IfTimelineItemIsNotFound(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithNotExistingTimelineItemyId();

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            var expectedErrorMessage = string.Format(
                ErrorMessages.EntityByIdNotFound,
                nameof(TimelineEntity),
                id);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);
            var actualErrorMessage = result.Errors[0].Message;

            // Assert
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ReturnsFailResult_IfSaveChangesAsyncNotSuccessful(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(id);
            _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ShouldLogCorrectErrorMessage_IfSaveChangesAsyncNotSuccessful(int id)
        {
            // Arrange
            MockRepositoryWrapperSetupWithExistingTimelineItemId(id);
            _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(0);

            var handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

            var expectedErrorMessage = string.Format(
                ErrorMessages.DeleteFailed,
                nameof(TimelineEntity),
                id);

            // Act
            var result = await handler.Handle(new DeleteTimelineItemCommand(id), CancellationToken.None);

            var actualErrorMessage = result.Errors[0].Message;

            // Assert
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        private static TimelineEntity GetTimelineItem(int id)
        {
            return new TimelineEntity
            {
                Id = id,
            };
        }

        private static TimelineEntity? GetTimelineItemWithNotExistingId()
        {
            return null;
        }

        private void MockRepositoryWrapperSetupWithExistingTimelineItemId(int id)
        {
            _mockRepositoryWrapper.Setup(x => x.TimelineRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineEntity>,
                    IIncludableQueryable<TimelineEntity, object>>>()))
                .ReturnsAsync(GetTimelineItem(id));

            _mockRepositoryWrapper.Setup(x => x.TimelineRepository
                .Delete(GetTimelineItem(id)));

            _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        }

        private void MockRepositoryWrapperSetupWithNotExistingTimelineItemyId()
        {
            _mockRepositoryWrapper.Setup(x => x.TimelineRepository
                .GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineEntity>,
                    IIncludableQueryable<TimelineEntity, object>>>()))
                .ReturnsAsync(GetTimelineItemWithNotExistingId());
        }
    }
}