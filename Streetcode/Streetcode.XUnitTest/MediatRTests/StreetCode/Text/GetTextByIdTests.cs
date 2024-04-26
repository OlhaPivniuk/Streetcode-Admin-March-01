namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Text;

using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

public class GetTextByIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    public GetTextByIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_ShouldReturnOk_IfIdExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_RepositoryShouldCallGetFirstOrDefaultAsyncOnlyOnce_IfTextExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            r =>
            r.TextRepository.GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Text, bool>>>(),
               It.IsAny<Func<IQueryable<Text>,
               IIncludableQueryable<Text, object>>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_MapperShouldCallMapOnlyOnce_IfTextExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            m => m.Map<TextDto>(It.IsAny<Text>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_ShouldReturnTextWithCorrectId_IfTextExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Value.Id);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_ShouldReturnTextDto_IfTextExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.IsType<TextDto>(result.Value);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_ShouldReturnFail_WhenTextIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetTextById_ShouldLogCorrectErrorMessage_WhenTextIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetTextByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expected = $"Cannot find any text with corresponding id: {id}";

        // Act
        var result = await handler.Handle(new GetTextByIdQuery(id), CancellationToken.None);
        var actual = result.Errors[0].Message;

        // Assert
        Assert.Equal(expected, actual);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<TextDto>(It.IsAny<Text>()))
            .Returns(new TextDto
            {
                Id = id,
                Title = "Text",
                TextContent = "TextContent",
                StreetcodeId = id,
                AdditionalText = "AdditionalText",
            });
    }

    private void MockRepositorySetupReturnsFact(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.TextRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Text, bool>>>(),
               It.IsAny<Func<IQueryable<Text>,
               IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync(new Text { Id = id });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.TextRepository
            .GetFirstOrDefaultAsync(
               It.IsAny<Expression<Func<Text, bool>>>(),
               It.IsAny<Func<IQueryable<Text>,
               IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync((Text?)null);
    }
}
