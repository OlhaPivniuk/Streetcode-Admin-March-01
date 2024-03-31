namespace Streetcode.XUnitTest.MediatRTests.Media.Image;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Image.GetAll;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetAllImageHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IBlobService> _mockBlobService;

    public GetAllImageHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockBlobService = new Mock<IBlobService>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ReturnsOkResult_WithListOfImages_WhenImagesFound()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsFailedResult_WhenImagesNotFound()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        MockMapperSetup();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_LogsError_WhenImagesNotFound()
    {
        // Arrange
        MockRepositorySetupReturnsNull();
        MockMapperSetup();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            logger => logger.LogError(
                It.IsAny<GetAllImagesQuery>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsCollectionOfCorrectCount_WhenImagesFound()
    {
        // Arrange
        var mockVideo = GetImageList();
        var expectedCount = mockVideo.Count;

        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async Task Handle_MapperShouldMapOnlyOnce_WhenImagesFound()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<ImageDto>>(It.IsAny<IEnumerable<Image>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCollectionOfImageDto_WhenImagesFound()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        Assert.IsType<List<ImageDto>>(result.Value);
    }

    [Fact]
    public async Task Handle_ReturnsFailedResult_WhenImagesAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ShouldLogCorrectErrorMessage_WhenImagesAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllImagesHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockBlobService.Object,
            _mockLogger.Object);

        var expectedError = string.Format(
                ErrorMessages.EntitiesNotFound,
                nameof(DAL.Entities.Media.Images.Image));

        // Act
        var result = await handler.Handle(new GetAllImagesQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    private static List<Image> GetImageList()
    {
        return new List<Image>
        {
            new ()
            {
                Id = 1,
                Base64 = "base64_encoded_data_1",
                BlobName = "image1.jpg",
                MimeType = "image/jpeg",
            },
            new ()
            {
                Id = 2,
                Base64 = "base64_encoded_data_2",
                BlobName = "image2.jpg",
                MimeType = "image/jpeg",
            },
            new ()
            {
                Id = 3,
                Base64 = "base64_encoded_data_3",
                BlobName = "image3.jpg",
                MimeType = "image/jpeg",
            },
        };
    }

    private static List<ImageDto> GetImageDtoList()
    {
        return new List<ImageDto>
        {
            new ()
            {
                Id = 1,
                BlobName = "image1.jpg",
                Base64 = "base64_encoded_data_1",
                MimeType = "image/jpeg",
            },
            new ()
            {
                Id = 2,
                BlobName = "image2.jpg",
                Base64 = "base64_encoded_data_2",
                MimeType = "image/jpeg",
            },
            new ()
            {
                Id = 3,
                BlobName = "image3.jpg",
                Base64 = "base64_encoded_data_3",
                MimeType = "image/jpeg",
            },
        };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<ImageDto>>(It.IsAny<IEnumerable<Image>>()))
            .Returns(GetImageDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.ImageRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<Func<IQueryable<Image>,
            IIncludableQueryable<Image, object>>>()))
            .ReturnsAsync(GetImageList());
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.ImageRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<Func<IQueryable<Image>,
            IIncludableQueryable<Image, object>>>()))
            .ReturnsAsync((IEnumerable<Image>?)null);
    }
}