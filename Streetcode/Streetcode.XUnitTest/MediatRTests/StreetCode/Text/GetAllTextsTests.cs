namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.Dto.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Xunit;
using Streetcode.DAL.Entities.Streetcode.TextContent;

public class GetAllTextsTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;
    public GetAllTextsTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAllTexts_ShouldReturnOk_WhenTextsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();
        var handle = new GetAllTextsHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handle.Handle(new GetAllTextsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllTexts_ShouldReturnCollectionOfCorrectCount_WhenFactsExist()
    {
        // Arrange
        var mockTexts = GetTextList();
        var expectedCount = mockTexts.Count;

        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handle = new GetAllTextsHandler(_mockRepositoryWrapper.Object, _mockMapper.Object, _mockLogger.Object);

        // Act
        var result = await handle.Handle(new GetAllTextsQuery(), CancellationToken.None);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async Task GetAllTexts_RepositoryShouldCallGetAllAsyncOnlyOnce_WhenTextsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllTextsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllTextsQuery(), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.TextRepository.GetAllAsync(null, null), Times.Once);
    }

    [Fact]
    public async Task GetAllTexts_MapperShouldMapOnlyOnce_WhenTextsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllTextsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllTextsQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<TextDto>>(It.IsAny<IEnumerable<Text>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllTexts_ShouldReturnCollectionOfTextDto_WhenTextsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllTextsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllTextsQuery(), CancellationToken.None);

        // Assert
        Assert.IsType<List<TextDto>>(result.Value);
    }

    [Fact]
    public async Task GetAllTexts_ShouldReturnFail_WhenTextsAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllTextsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllTextsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetAllTexts_ShouldLogCorrectErrorMessage_WhenTextsAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllTextsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedError = "Cannot find any text";

        // Act
        var result = await handler.Handle(new GetAllTextsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    private static List<Text> GetTextList()
    {
        return new List<Text>
            {
                new()
                {
                    Id = 1,
                    Title = "Title1",
                    TextContent = "Text1",
                    StreetcodeId = 1,
                    AdditionalText = "AdditionalText1",
                },
                new()
                {
                    Id = 2,
                    Title = "Title2",
                    TextContent = "Text2",
                    StreetcodeId = 2,
                    AdditionalText = "AdditionalText2",
                },
                new()
                {
                    Id = 3,
                    Title = "Title3",
                    TextContent = "Text3",
                    StreetcodeId = 3,
                    AdditionalText = "AdditionalText3",
                }
            };
    }

    private static List<TextDto> GetTextDtoList()
    {
        return new List<TextDto>
            {
                new()
                {
                    Id = 1,
                    Title = "Title1",
                    TextContent = "Text1",
                    StreetcodeId = 1,
                    AdditionalText = "AdditionalText1",
                },
                new()
                {
                    Id = 2,
                    Title = "Title2",
                    TextContent = "Text2",
                    StreetcodeId = 2,
                    AdditionalText = "AdditionalText2",
                },
                new()
                {
                    Id = 3,
                    Title = "Title3",
                    TextContent = "Text3",
                    StreetcodeId = 3,
                    AdditionalText = "AdditionalText3",
                }
            };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<TextDto>>(It.IsAny<IEnumerable<Text>>()))
            .Returns(GetTextDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.TextRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Text, bool>>>(),
                It.IsAny<Func<IQueryable<Text>,
            IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync(GetTextList());
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.TextRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Text, bool>>>(),
                It.IsAny<Func<IQueryable<Text>,
            IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync((IEnumerable<Text>?)null);
    }
}
