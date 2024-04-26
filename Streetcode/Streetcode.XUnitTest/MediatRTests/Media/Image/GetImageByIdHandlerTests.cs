namespace Streetcode.XUnitTest.MediatRTests.Media.Image;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Dto.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Image.GetById;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetImageByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
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
        await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.ImageRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Image, bool>>>(),
               It.IsAny<Func<IQueryable<Image>,
               IIncludableQueryable<Image, object>>>()),
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
        await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<ImageDto>(It.IsAny<Image>()),
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
            _mockLogger.Object);

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
        Assert.IsType<ImageDto>(result.Value);
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

        var expectedMessage = string.Format(
               ErrorMessages.EntityByIdNotFound,
               nameof(DAL.Entities.Media.Images.Image),
               id);

        // Act
        var result = await handler.Handle(new GetImageByIdQuery(id), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<ImageDto>(It.IsAny<Image>()))
            .Returns(new ImageDto { Id = id });
    }

    private void MockRepositorySetupReturnsImage(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.ImageRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Image, bool>>>(),
               It.IsAny<Func<IQueryable<Image>,
               IIncludableQueryable<Image, object>>>()))
            .ReturnsAsync(new Image { Id = id });
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
