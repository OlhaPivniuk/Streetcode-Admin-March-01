namespace Streetcode.XUnitTest.MediatRTests.Media.Art;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetArtByIdTest
{
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;

    public GetArtByIdTest()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsOkResult_WhenIdExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsArt(id);
        MockMapperSetup(id);

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryCallGetFirstOrDefaultAsyncOnlyOnce_WhenArtExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsArt(id);
        MockMapperSetup(id);

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.ArtRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Art, bool>>>(),
               It.IsAny<Func<IQueryable<Art>,
               IIncludableQueryable<Art, object>>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_MapperCallMapOnlyOnce_WhenArtExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsArt(id);
        MockMapperSetup(id);

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<ArtDto>(It.IsAny<Art>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnsArtWithCorrectId_WhenArtExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsArt(id);
        MockMapperSetup(id);

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Value.Id);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnArtDto_WhenArtExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsArt(id);
        MockMapperSetup(id);

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.IsType<ArtDto>(result.Value);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ReturnFail_WhenArtIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldLogCorrectErrorMessage_WhenArtIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetArtByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedMessage = string.Format(ErrorMessages.EntityByIdNotFound, nameof(DAL.Entities.Media.Images.Art), id);

        // Act
        var result = await handler.Handle(new GetArtByIdQuery(id), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<ArtDto>(It.IsAny<Art>()))
            .Returns(new ArtDto { Id = id });
    }

    private void MockRepositorySetupReturnsArt(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.ArtRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Art, bool>>>(),
               It.IsAny<Func<IQueryable<Art>,
               IIncludableQueryable<Art, object>>>()))
            .ReturnsAsync(new Art { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.ArtRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Art, bool>>>(),
                It.IsAny<Func<IQueryable<Art>,
            IIncludableQueryable<Art, object>>>()))
            .ReturnsAsync((IEnumerable<Art>?)null);
    }
}
