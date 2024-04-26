using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Streetcode;
using FluentResults;

using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.MediatRTests.Media.Video.Create
{
    public class CreateVideoHandlerTests
    {
        private const int SUCCESSFULSAVE = 1;
        private const int FAILEDSAVE = -1;
        private const int EXISTINGID = 1;

        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CreateVideoHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ValidCreateVideoCommand_ShouldReturnSuccessResult()
        {
            // Arrange
            var request = GetValidCreateVideoRequest();
            SetupMock(request, SUCCESSFULSAVE);
            var handler = CreateHandler();
            var command = new CreateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_InvalidCreateVideoCommand_ShouldReturnFailedResult()
        {
            // Arrange
            var request = GetValidCreateVideoRequest();
            SetupMock(request, FAILEDSAVE);
            var handler = CreateHandler();
            var command = new CreateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ValidCreateVideoCommand_ShouldReturnCorrectResultType()
        {
            // Arrange
            var request = GetValidCreateVideoRequest();
            SetupMock(request, SUCCESSFULSAVE);
            var handler = CreateHandler();
            var command = new CreateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.Should().BeOfType<Result<CreateVideoRequestDto>>();
        }

        [Fact]
        public async Task Handle_AnyCreateVideoCommand_WithAnyStreetcode_ShouldReturnResultOfCorrectType()
        {
            // Arrange
            var request = GetValidCreateVideoRequest();
            var expectedType = typeof(Result<CreateVideoRequestDto>);
            SetupMock(request, SUCCESSFULSAVE);
            var handler = CreateHandler();
            var command = new CreateVideoCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.Should().BeOfType(expectedType);
        }

        private static CreateVideoRequestDto GetValidCreateVideoRequest()
        {
            return new CreateVideoRequestDto(
                Title: "Test Video",
                Description: "Description for test video",
                Url: "https://www.youtube.com/watch?v=video1",
                StreetcodeId: EXISTINGID);
        }

        private CreateVideoHandler CreateHandler()
        {
            return new CreateVideoHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        private void SetupMock(CreateVideoRequestDto request, int saveChangesAsyncResult)
        {
            var streetcode = request.StreetcodeId switch
            {
                EXISTINGID => new StreetcodeContent { Id = request.StreetcodeId },
                _ => null,
            };

            var videoEntity = new VideoEntity
            {
                Id = 1,
            };

            _mockRepositoryWrapper
                .Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
                    AnyEntityPredicate<StreetcodeContent>(),
                    AnyEntityInclude<StreetcodeContent>()))
                .ReturnsAsync(streetcode);

            _mockRepositoryWrapper
                .Setup(r => r.VideoRepository.Create(It.IsAny<VideoEntity>()))
                .Returns(videoEntity);

            _mockRepositoryWrapper
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(saveChangesAsyncResult);

            _mockMapper
                .Setup(m => m.Map<VideoEntity>(It.Is<object>(obj => obj is CreateVideoRequestDto)))
                .Returns(videoEntity);
        }

        private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
        {
            return It.IsAny<Expression<Func<TEntity, bool>>>();
        }

        private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
        {
            return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
        }
    }
}
