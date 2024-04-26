using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;
using ImageEntity = Streetcode.DAL.Entities.Media.Images.Image;
using StreetcodeArtEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeArt;
namespace Streetcode.XUnitTest.MediatRTests.Media.Art;

public class CreateArtHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;
    private const int EXISTINGID = 1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateArtHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentImageId()
    {
        // Arrange
        var request = new CreateArtRequestDto(
            ImageId: int.MaxValue,
            StreetcodeId: 1,
            Title: "title",
            Description: "description");

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(ImageEntity).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateArtCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeId()
    {
        // Arrange
        var request = new CreateArtRequestDto(
            ImageId: 1,
            StreetcodeId: int.MaxValue,
            Title: "title",
            Description: "description");

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(StreetcodeContent),
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateArtCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreateArtCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    private static CreateArtRequestDto GetValidCreateRequest()
    {
        return new CreateArtRequestDto(
            ImageId: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: "descrption");
    }

    private CreateArtHandler CreateHandler()
    {
        return new CreateArtHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateArtRequestDto request, int saveChangesAsyncResult)
    {
        var image = request.ImageId switch
        {
            EXISTINGID => new ImageEntity { Id = request.ImageId },
            _ => null,
        };

        var streetcode = request.StreetcodeId switch
        {
            EXISTINGID => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        var entity = new ArtEntity { Id = 1 };

        var streetcodeArt = new StreetcodeArtEntity
        {
            StreetcodeId = 1,
            ArtId = 1,
        };
        var responseDto = new CreateArtResponseDto(
            Id: 1,
            Description: request.Description,
            Title: request.Title,
            ImageId: request.ImageId,
            StreetcodeId: request.StreetcodeId);

        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeArtRepository.GetFirstOrDefaultAsync(
            AnyEntityPredicate<StreetcodeArtEntity>(),
            AnyEntityInclude<StreetcodeArtEntity>()))
            .ReturnsAsync(streetcodeArt);

        _mockRepositoryWrapper
                .Setup(repo => repo.ImageRepository.GetFirstOrDefaultAsync(
                    AnyEntityPredicate<ImageEntity>(),
                    AnyEntityInclude<ImageEntity>()))
                .ReturnsAsync(image);

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeContent>(),
                AnyEntityInclude<StreetcodeContent>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper.Setup(repo => repo.ArtRepository.Create(
            It.IsAny<ArtEntity>())).Returns(entity);

        _mockRepositoryWrapper.Setup(repo => repo.StreetcodeArtRepository.Create(
            It.IsAny<StreetcodeArtEntity>())).Returns(streetcodeArt);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<CreateArtRequestDto, ArtEntity>(It.IsAny<CreateArtRequestDto>())).Returns(entity);

        _mockMapper
            .Setup(m => m.Map<ArtEntity, CreateArtResponseDto>(It.IsAny<ArtEntity>())).Returns(responseDto);
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
