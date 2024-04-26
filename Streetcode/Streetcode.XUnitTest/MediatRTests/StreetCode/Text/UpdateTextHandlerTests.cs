using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.Update;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Text;

public class UpdateTextHandlerTests
{
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;
    private const int MINLENGTH = 1;
    private const string PREFILLEDTEXT = "Текст підготовлений спільно з ";

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public UpdateTextHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidUpdateTextRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNullInAdditionalTextProperty_IfAdditionalTextWasNull()
    {
        // Arrange
        var request = new UpdateTextRequestDto(
            Id: 1,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: null);

        var handler = CreateHandler();
        var command = new UpdateTextCommand(request);

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
        var request = new UpdateTextRequestDto(
            Id: 1,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: new string('a', MINLENGTH));

        var handler = CreateHandler();
        var command = new UpdateTextCommand(request);
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
        var request = GetValidUpdateTextRequest();
        var expectedType = typeof(Result<UpdateTextResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidUpdateTextRequest();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new UpdateTextCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidUpdateTextRequest();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new UpdateTextCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private UpdateTextHandler CreateHandler()
    {
        return new UpdateTextHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(UpdateTextRequestDto request, int saveChangesAsyncResult)
    {
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
            null => new UpdateTextResponseDto(
                Id: 1,
                StreetcodeId: 1,
                Title: new string('a', MINLENGTH),
                TextContent: new string('a', MINLENGTH),
                AdditionalText: null),
            _ => new UpdateTextResponseDto(
                Id: 1,
                StreetcodeId: 1,
                Title: new string('a', MINLENGTH),
                TextContent: new string('a', MINLENGTH),
                AdditionalText: PREFILLEDTEXT + new string('a', MINLENGTH))
        };

        _mockRepositoryWrapper.Setup(repo => repo.TextRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<TextEntity>(),
                AnyEntityInclude<TextEntity>()))
            .ReturnsAsync(text);

        _mockRepositoryWrapper.Setup(repo => repo.TextRepository.Update(
            It.IsAny<TextEntity>())).Returns(It.IsAny<EntityEntry<TextEntity>>());

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<TextEntity>(It.IsAny<UpdateTextRequestDto>())).Returns(text);

        _mockMapper
           .Setup(m => m.Map<UpdateTextResponseDto>(It.IsAny<TextEntity>())).Returns(dtoResponseText);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static UpdateTextRequestDto GetValidUpdateTextRequest()
    {
        return new(
            Id: 1,
            Title: new string('a', MINLENGTH),
            TextContent: new string('a', MINLENGTH),
            AdditionalText: new string('a', MINLENGTH));
    }
}