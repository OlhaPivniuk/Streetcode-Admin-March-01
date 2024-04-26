using System.Linq.Expressions;
using Moq;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using AutoMapper;
using Streetcode.BLL.DTO.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Entities.Sources;
using Streetcode.BLL.Dto.Sources;
using Streetcode.BLL.Resources.Errors;
using Xunit;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using FluentAssertions;
using FluentResults;

namespace Streetcode.XUnitTest.MediatRTests.Sources;

public class UpdateCategoryContentHandlerTests
{
    private const int SUCCESSFULRESULT = 1;
    private const int FAILURERESULT = -1;

    private const int EXISTSSTREETCODEID = 1;
    private const int NOTEXISTSSTREETCODEID = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    public UpdateCategoryContentHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenSaveChangesAsyncWorkedCorrect()
    {
        // Arrange
        var request = new CategoryContentUpdateDto("Text", 1, 1);
        SetupMockRepository(request, SUCCESSFULRESULT);
        SetupMockMapper();
        var command = new UpdateCategoryContentCommand(request);
        var handler = new UpdateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenSaveChangesAsyncWorkedIncorrect()
    {
        // Arrange
        var request = new CategoryContentUpdateDto("Text", 1, 1);
        SetupMockRepository(request, FAILURERESULT);
        SetupMockMapper();
        var command = new UpdateCategoryContentCommand(request);
        var handler = new UpdateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFail_WhenStreetcodeNotFound()
    {
        // Arrange
        var request = new CategoryContentUpdateDto("Text", 1, NOTEXISTSSTREETCODEID);
        SetupMockRepository(request, SUCCESSFULRESULT);
        SetupMockMapper();
        var command = new UpdateCategoryContentCommand(request);
        var handler = new UpdateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnResultOfExpectedType_WithCorrectConditions()
    {
        // Arrange
        var request = new CategoryContentUpdateDto("Text", 1, 1);
        SetupMockRepository(request, SUCCESSFULRESULT);
        SetupMockMapper();
        var command = new UpdateCategoryContentCommand(request);
        var handler = new UpdateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType(typeof(Result<StreetcodeCategoryContentDto>));
    }

    [Fact]
    public async Task Handle_Should_ReturnExpectedErrorMessage_WhenStreetcodeContentDoesNotExist()
    {
        // Arrange
        var request = new CategoryContentUpdateDto("Text", 1, NOTEXISTSSTREETCODEID);
        SetupMockRepository(request, SUCCESSFULRESULT);
        SetupMockMapper();
        var command = new UpdateCategoryContentCommand(request);
        var handler = new UpdateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == StreetcodeNotFoundErrorMessage(request));
    }

    [Fact]
    public async Task Handle_Should_ReturnExpectedErrorMessage_WhenUpdateFailed()
    {
        // Arrange
        var request = new CategoryContentUpdateDto("Text", 1, 1);
        SetupMockRepository(request, FAILURERESULT);
        SetupMockMapper();
        var command = new UpdateCategoryContentCommand(request);
        var handler = new UpdateCategoryContentHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == UpdateFailedErrorMessage(request));
    }

    private void SetupMockRepository(CategoryContentUpdateDto request, int resultCase)
    {
        var streetcode = request.StreetcodeId switch
        {
            EXISTSSTREETCODEID => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        _mockRepositoryWrapper.Setup(x => x.StreetcodeRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper.Setup(x => x.StreetcodeCategoryContentRepository.Update(GetStreetcodeCategoryContent()));

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(resultCase);
    }

    private void SetupMockMapper()
    {
        _mockMapper.Setup(x => x.Map<StreetcodeCategoryContent>(It.IsAny<CategoryContentUpdateDto>()))
            .Returns(GetStreetcodeCategoryContent());

        _mockMapper.Setup(x => x.Map<StreetcodeCategoryContentDto>(It.IsAny<StreetcodeCategoryContent>()))
            .Returns(GetStreetcodeCategoryContentDto());
    }

    private static StreetcodeCategoryContent GetStreetcodeCategoryContent()
    {
        return new StreetcodeCategoryContent { StreetcodeId = 1 };
    }

    private static StreetcodeCategoryContentDto GetStreetcodeCategoryContentDto()
    {
        return new StreetcodeCategoryContentDto { StreetcodeId = 1 };
    }

    private string StreetcodeNotFoundErrorMessage(CategoryContentUpdateDto request)
    {
        return string.Format(
            ErrorMessages.EntityByCategoryIdNotFound,
            nameof(StreetcodeContent),
            request.SourceLinkCategoryId);
    }

    private string UpdateFailedErrorMessage(CategoryContentUpdateDto request)
    {
       return string.Format(
            ErrorMessages.UpdateFailed,
            nameof(StreetcodeCategoryContent),
            request.SourceLinkCategoryId);
    }
}