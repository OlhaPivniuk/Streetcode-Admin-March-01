using System.Linq.Expressions;
using Moq;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using Xunit;
using AutoMapper;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using FluentAssertions;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Entities.Streetcode;

using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact
{
    public class UpdateFactHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        public UpdateFactHandlerTests()
        {
            _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
            _mockLogger = new Mock<ILoggerService>();
            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenSaveChangesAsyncWorkedSuccessfully()
        {
            // Arrange
            var request = GetRequest();
            var fact = GetFact();

            MockMapperSetup(request, fact);
            MockRepositoryWrapperSetup(request);
            SaveChangesAsyncWorkedSuccessfullySetup();

            var command = new UpdateFactCommand(request);
            var handler = new UpdateFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object, _mockMapper.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_ReturnFail_WhenSaveChangesAsyncWorkedIncorrectly()
        {
            // Arrange
            var request = GetRequest();
            var fact = GetFact();

            MockMapperSetup(request, fact);
            MockRepositoryWrapperSetup(request);
            SaveChangesAsyncWorkedFailSetup();

            var command = new UpdateFactCommand(request);
            var handler = new UpdateFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object, _mockMapper.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_ReturnCorrectError_WhenSaveChangesAsyncWorkedIncorrectly()
        {
            // Arrange
            var request = GetRequest();
            var fact = GetFact();

            MockMapperSetup(request, fact);
            MockRepositoryWrapperSetup(request);
            SaveChangesAsyncWorkedFailSetup();

            var command = new UpdateFactCommand(request);
            var handler = new UpdateFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object, _mockMapper.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Errors.Should().Contain(error => error.Message == ExpectedErrorMessage(request));
        }

        private void MockMapperSetup(UpdateFactDto request, FactEntity fact)
        {
            _mockMapper.Setup(mapper => mapper
            .Map<FactEntity>(It.IsAny<UpdateFactDto>()))
            .Returns(fact);
            _mockMapper.Setup(mapper => mapper
            .Map<FactDto>(It.IsAny<FactEntity>()))
            .Returns(GetFactDto());
        }

        private void SaveChangesAsyncWorkedSuccessfullySetup()
        {
            _mockRepositoryWrapper.Setup(repository => repository.SaveChangesAsync())
                .ReturnsAsync(1);
        }

        private void MockRepositoryWrapperSetup(UpdateFactDto request)
        {
            var image = request.ImageId switch
            {
                1 => new Image { Id = request.ImageId },
                _ => null,
            };

            var streetcode = request.StreetcodeId switch
            {
                1 => new StreetcodeContent { Id = request.StreetcodeId },
                _ => null,
            };
            _mockRepositoryWrapper.Setup(repository => repository
            .FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>()))
                .ReturnsAsync(GetFact());

            _mockRepositoryWrapper.Setup(repository => repository.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<Func<IQueryable<Image>, IIncludableQueryable<Image, object>>>()))
                .ReturnsAsync(image);

            _mockRepositoryWrapper.Setup(repository => repository.StreetcodeRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
                .ReturnsAsync(streetcode);

            _mockRepositoryWrapper.Setup(repository => repository.FactRepository.Update(It.IsAny<FactEntity>()));
        }

        private void SaveChangesAsyncWorkedFailSetup()
        {
            _mockRepositoryWrapper.Setup(repository => repository.SaveChangesAsync())
                .ReturnsAsync(-1);
        }

        private static UpdateFactDto GetRequest()
        {
            return new UpdateFactDto(1, "Title", "Content", 1, 1);
        }

        private static FactEntity GetFact()
        {
            return new FactEntity()
            {
                Id = 1,
                Title = "Title",
                FactContent = "Content",
                ImageId = 1,
                StreetcodeId = 1,
            };
        }

        private static FactDto GetFactDto()
        {
            return new FactDto(1, 1, "Title", "Content", 1, 1);
        }

        private static string ExpectedErrorMessage(UpdateFactDto request)
        {
           return string.Format(
                ErrorMessages.UpdateFailed,
                nameof(FactEntity),
                request.Id);
        }
    }
}