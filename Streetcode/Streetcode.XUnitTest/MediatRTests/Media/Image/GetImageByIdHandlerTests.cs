﻿using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Image.GetAll;
using Streetcode.BLL.MediatR.Media.Image.GetById;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Media.Image
{
    public class GetImageByIdHandlerTests
    {
        private Mock<IRepositoryWrapper> _mockRepositoryWrapper;
        private Mock<IMapper> _mockMapper;
        private readonly Mock<ILoggerService> _mockLogger;
        private readonly Mock<IBlobService> _mockBlobService;

        public GetImageByIdHandlerTests()
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
            MockRepositorySetupReturnsImage(id);
            MockMapperSetup(id);

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_RepositoryCallGetFirstOrDefaultAsyncOnlyOnce_WhenImageExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsImage(id);
            MockMapperSetup(id);

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

            // Assert
            _mockRepositoryWrapper.Verify(
                repo =>
                repo.ImageRepository.GetFirstOrDefaultAsync(
                   It.IsAny<Expression<Func<Streetcode.DAL.Entities.Media.Images.Image, bool>>>(),
                   It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.Media.Images.Image>,
                   IIncludableQueryable<Streetcode.DAL.Entities.Media.Images.Image, object>>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_MapperCallMapOnlyOnce_WhenImageExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsImage(id);
            MockMapperSetup(id);

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

            // Assert
            _mockMapper.Verify(
                mapper => mapper.Map<ImageDTO>(It.IsAny<Streetcode.DAL.Entities.Media.Images.Image>()),
                Times.Once);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ReturnsImageWithCorrectId_WhenImageExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsImage(id);
            MockMapperSetup(id);

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object); ;

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

            // Assert
            Assert.Equal(id, result.Value.Id);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ReturnImageDto_WhenImageExists(int id)
        {
            // Arrange
            MockRepositorySetupReturnsImage(id);
            MockMapperSetup(id);

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

            // Assert
            Assert.IsType<ImageDTO>(result.Value);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ReturnFail_WhenImageIsNotFound(int id)
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object);

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
        }

        [Theory]
        [InlineData(1)]
        public async Task Handle_ShouldLogCorrectErrorMessage_WhenImageIsNotFound(int id)
        {
            // Arrange
            MockRepositorySetupReturnsNull();

            var handler = new GetImageByIdHandler(
                _mockRepositoryWrapper.Object,
                _mockMapper.Object,
                _mockBlobService.Object,
                _mockLogger.Object);

            var expectedMessage = $"Cannot find a image with corresponding id: {id}";

            // Act
            var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);
            var actualMessage = result.Errors.First().Message;

            // Assert
            Assert.Equal(expectedMessage, actualMessage);
        }

        private void MockMapperSetup(int id)
        {
            _mockMapper.Setup(x => x
                .Map<ImageDTO>(It.IsAny<Streetcode.DAL.Entities.Media.Images.Image>()))
                .Returns(new ImageDTO { Id = id });
        }

        private void MockRepositorySetupReturnsImage(int id)
        {
            _mockRepositoryWrapper.Setup(x => x.ImageRepository
                .GetFirstOrDefaultAsync(
                   It.IsAny<Expression<Func<Streetcode.DAL.Entities.Media.Images.Image, bool>>>(),
                   It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.Media.Images.Image>,
                   IIncludableQueryable<Streetcode.DAL.Entities.Media.Images.Image, object>>>()))
                .ReturnsAsync(new Streetcode.DAL.Entities.Media.Images.Image { Id = id });
        }

        private void MockRepositorySetupReturnsNull()
        {
            _mockRepositoryWrapper.Setup(x => x.ImageRepository
                .GetAllAsync(
                    It.IsAny<Expression<Func<Streetcode.DAL.Entities.Media.Images.Image, bool>>>(),
                    It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.Media.Images.Image>,
                IIncludableQueryable<Streetcode.DAL.Entities.Media.Images.Image, object>>>()))
                .ReturnsAsync((IEnumerable<Streetcode.DAL.Entities.Media.Images.Image>?)null);
        }
    }

}