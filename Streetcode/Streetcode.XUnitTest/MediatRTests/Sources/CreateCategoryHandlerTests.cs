using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Sources;

public class CreateCategoryHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateCategoryHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateCategoryRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCategoryCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentImageId()
    {
        // Arrange
        var request = new CreateCategoryRequestDto(
            Title: "Title",
            ImageId: int.MaxValue,
            StreetcodeId: 1,
            Text: "text");

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(Image),
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateCategoryCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeId()
    {
        // Arrange
        var request = new CreateCategoryRequestDto(
            Title: "Title",
            ImageId: 1,
            StreetcodeId: int.MaxValue,
            Text: "text");

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(StreetcodeContent),
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateCategoryCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateCategoryRequest();
        var expectedType = typeof(Result<SourceLinkCategoryDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCategoryCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateCategoryRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreateCategoryCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncTwice_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateCategoryRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateCategoryCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    private CreateCategoryHandler CreateHandler()
    {
        return new CreateCategoryHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateCategoryRequestDto request, int saveChangesAsyncResult)
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

        var category = new SourceLinkCategory { Id = 1 };

        _mockRepositoryWrapper
                .Setup(repo => repo.ImageRepository.GetFirstOrDefaultAsync(
                    AnyEntityPredicate<Image>(),
                    AnyEntityInclude<Image>()))
                .ReturnsAsync(image);

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeContent>(),
                AnyEntityInclude<StreetcodeContent>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper.Setup(repo => repo.SourceCategoryRepository.Create(
            It.IsAny<SourceLinkCategory>())).Returns(category);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<SourceLinkCategory>(It.IsAny<CreateCategoryRequestDto>())).Returns(category);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreateCategoryRequestDto GetValidCreateCategoryRequest()
    {
        return new(Title: "Title",
            ImageId: 1,
            StreetcodeId: 1,
            Text: "text");
    }
}
