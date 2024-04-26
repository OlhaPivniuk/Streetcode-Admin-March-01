using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;
using ImageEntity = Streetcode.DAL.Entities.Media.Images.Image;
using StreetcodeEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.StreetcodeTests
{
    public class CreateStreetcodeHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;

        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public CreateStreetcodeHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILoggerService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnError_IfInvalidAudioFile()
        {
            // Arrange
            var request = GetValidCreateStreetcodeRequest();
            SetupMock(request, audioId: 1, url: "test-url", imageIds: new List<int> { 1 }, isAudioValid: false);
            var handler = CreateHandler();
            var command = new CreateStreetcodeCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnError_IfNonUniqueUrl()
        {
            // Arrange
            var request = GetValidCreateStreetcodeRequest();
            SetupMock(request, audioId: 1, url: "test-url", imageIds: new List<int> { 1 }, isUrlUnique: false);
            var handler = CreateHandler();
            var command = new CreateStreetcodeCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnError_IfInvalidImageFiles()
        {
            // Arrange
            var request = GetValidCreateStreetcodeRequest();
            SetupMock(request, audioId: 1, url: "test-url", imageIds: new List<int> { 1 }, hasJpeg: false, hasGif: false);
            var handler = CreateHandler();
            var command = new CreateStreetcodeCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnError_IfSaveChangesFailed()
        {
            // Arrange
            var request = GetValidCreateStreetcodeRequest();
            SetupMock(request, audioId: 1, url: "test-url", imageIds: new List<int> { 1 }, saveChangesResult: -1);
            var handler = CreateHandler();
            var command = new CreateStreetcodeCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnError_IfInternalErrorOccursDuringSave()
        {
            // Arrange
            var request = GetValidCreateStreetcodeRequest();
            SetupMock(request, audioId: 1, url: "test-url", imageIds: new List<int> { 1 }, saveChangesResult: -1);
            var handler = CreateHandler();
            var command = new CreateStreetcodeCommand(request);

            // Act
            var result = await handler.Handle(command, _cancellationToken);

            // Assert
            result.Errors.Single().Message.Should().Be("The images must contain at least one .gif and one .jpeg file.");
        }

        private CreateStreetcodeHandler CreateHandler()
        {
            return new CreateStreetcodeHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        private void SetupMock(
            CreateStreetcodeRequestDto request,
            int audioId,
            string url,
            List<int> imageIds,
            bool isAudioValid = true,
            bool isUrlUnique = true,
            bool hasJpeg = true,
            bool hasGif = true,
            int saveChangesResult = 1)
        {
            var streetcode = new StreetcodeEntity();

            _mockMapper
                .Setup(m => m.Map<CreateStreetcodeRequestDto, StreetcodeEntity>(request))
                .Returns(streetcode);

            _mockRepositoryWrapper
                .Setup(repo => repo.StreetcodeRepository.Create(streetcode));

            _mockRepositoryWrapper.Setup(r => r.AudioRepository.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<AudioEntity, bool>>>(),
                null))
                .ReturnsAsync(isAudioValid ? new AudioEntity() : null);

            _mockRepositoryWrapper
                .Setup(r => r.StreetcodeRepository.GetSingleOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeEntity, bool>>>(), null))
                .ReturnsAsync(isUrlUnique ? null : new StreetcodeEntity());

            _mockRepositoryWrapper.Setup(r => r.ImageRepository.GetSingleOrDefaultAsync(
                It.IsAny<Expression<Func<ImageEntity, bool>>>(),
                null))
                .ReturnsAsync(hasJpeg ? new ImageEntity() { MimeType = "image/jpeg" } : null);

            _mockRepositoryWrapper
                .Setup(r => r.ImageRepository.FindAll(
                 It.IsAny<Expression<Func<ImageEntity, bool>>>()))
                .Returns(imageIds.Select(id => new ImageEntity { Id = id, MimeType = hasGif ? "image/gif" : "image/jpeg" }).AsQueryable());

            _mockRepositoryWrapper
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(saveChangesResult);
        }

        private static CreateStreetcodeRequestDto GetValidCreateStreetcodeRequest()
        {
            return new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                EventStartOrPersonBirthDate = DateTime.Now,
                TransliterationUrl = "validurl",
                TagIds = Enumerable.Range(1, 5),
                ImageIds = Enumerable.Range(1, 2)
            };
        }
    }
}