using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Text;

public class CreateTextHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;
    private const int MINLENGTH = 1;
    private const string PREFILLEDTEXT = "Текст підготовлений спільно з ";

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateTextHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreateTextRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentStreetcodeId()
    {
        // Arrange
        var request = new CreateTextRequestDto(
            StreetcodeId: int.MaxValue,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: new string('a', MINLENGTH));

        var expectedErrorMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            typeof(StreetcodeContent).Name,
            int.MaxValue);

        SetupMock(request, SUCCESSFULSAVE);

        var handler = CreateHandler();
        var command = new CreateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnNullInAdditionalTextProperty_IfAdditionalTextWasNull()
    {
        // Arrange
        var request = new CreateTextRequestDto(
            StreetcodeId: 1,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: null);

        var handler = CreateHandler();
        var command = new CreateTextCommand(request);

        SetupMock(request, SUCCESSFULSAVE);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Null(result.Value.AdditionalText);
    }

    [Fact]
    public async Task Handle_ShouldContainPREFILLEDTEXTinAdditionalTextProperty_IfAdditionalTextWasNotNull()
    {
        // Arrange
        var request = new CreateTextRequestDto(
            StreetcodeId: 1,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: new string('a', MINLENGTH));

        var handler = CreateHandler();
        var command = new CreateTextCommand(request);
        string expextedAdditionalText = PREFILLEDTEXT + request.AdditionalText;

        SetupMock(request, SUCCESSFULSAVE);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal(expextedAdditionalText, result.Value.AdditionalText);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateTextRequest();
        var expectedType = typeof(Result<CreateTextResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreateTextRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreateTextRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreateTextCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private CreateTextHandler CreateHandler()
    {
        return new CreateTextHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreateTextRequestDto request, int saveChangesAsyncResult)
    {
        var streetcode = request.StreetcodeId switch
        {
            1 => new StreetcodeContent { Id = request.StreetcodeId },
            _ => null,
        };

        var text = request.AdditionalText switch
        {
            null => new TextEntity
            {
                Id = 1,
                StreetcodeId = 1,
                Title = new string('a', MINLENGTH),
                TextContent = new string('a', MINLENGTH),
                AdditionalText = null
            },
            _ => new TextEntity
            {
                Id = 1,
                StreetcodeId = 1,
                Title = new string('a', MINLENGTH),
                TextContent = new string('a', MINLENGTH),
                AdditionalText = new string('a', MINLENGTH)
            }
        };

        var dtoResponseText = request.AdditionalText switch
        {
            null => new CreateTextResponseDto(
                Id: 1,
                StreetcodeId: 1,
                Title: new string('a', MINLENGTH),
                TextContent: new string('a', MINLENGTH),
                AdditionalText: null),
            _ => new CreateTextResponseDto(
                Id: 1,
                StreetcodeId: 1,
                Title: new string('a', MINLENGTH),
                TextContent: new string('a', MINLENGTH),
                AdditionalText: PREFILLEDTEXT + new string('a', MINLENGTH))
        };

        _mockRepositoryWrapper
            .Setup(repo => repo.StreetcodeRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<StreetcodeContent>(),
                AnyEntityInclude<StreetcodeContent>()))
            .ReturnsAsync(streetcode);

        _mockRepositoryWrapper.Setup(repo => repo.TextRepository.Create(
            It.IsAny<TextEntity>())).Returns(text);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<TextEntity>(It.IsAny<CreateTextRequestDto>())).Returns(text);
        _mockMapper
            .Setup(m => m.Map<CreateTextResponseDto>(It.IsAny<TextEntity>())).Returns(dtoResponseText);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreateTextRequestDto GetValidCreateTextRequest()
    {
        return new(
            StreetcodeId: 1,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: new string('a', MINLENGTH));
    }
}