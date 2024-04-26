namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.BLL.Resources.Errors;

public class GetFactByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetFactByStreetcodeIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldReturnOk_WhenFactsWithThisStreetcodeIdExist(int streetcodeId)
    {
        // Arrange
        MockRepositorySetupReturnsData(streetcodeId);
        MockMapperSetup();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldReturnCollectionOfCorrectCount_WhenFactsWithThisStreetcodeIdExist(int streetcodeId)
    {
        // Arrange
        var mockFacts = GetFactsWithMatchingStreetcodeId(streetcodeId);
        var expectedCount = mockFacts.Count;

        MockRepositorySetupReturnsData(streetcodeId);
        MockMapperSetup();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_MapperShouldMapOnlyOnce_WhenFactsWithThisStreetcodeIdExist(int streetcodeId)
    {
        // Arrange
        MockRepositorySetupReturnsData(streetcodeId);
        MockMapperSetup();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetAllFacts_RepositoryShouldCallGetAllAsyncOnlyOnce_WhenFactsExist(int streetcodeId)
    {
        // Arrange
        MockRepositorySetupReturnsData(streetcodeId);
        MockMapperSetup();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.FactRepository.GetAllAsync(It.IsAny<Expression<Func<Fact, bool>>>(), null), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetAllFacts_ShouldReturnCollectionOfFactDto_WhenFactsExist(int streetcodeId)
    {
        // Arrange
        MockRepositorySetupReturnsData(streetcodeId);
        MockMapperSetup();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

        // Assert
        Assert.IsType<List<FactDto>>(result.Value);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldReturnFail_WhenFactsWithThisStreetcodeIdNotFound(int streetcodeId)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldReturnCorrectErrorMessage_WhenFactsWithThisStreetcodeIdNotFound(int streetcodeId)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetFactByStreetcodeIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(Fact),
            streetcodeId);

        // Act
        var result = await handler.Handle(new GetFactByStreetcodeIdQuery(streetcodeId), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private static List<Fact> GetFactsWithMatchingStreetcodeId(int id)
    {
        var facts = new List<Fact>
        {
            new () { Id = 1, StreetcodeId = 1, },
            new () { Id = 2, StreetcodeId = 1, },
            new () { Id = 3, StreetcodeId = 2, },
        };

        return facts.Where(f => f.StreetcodeId == id).ToList();
    }

    private static List<FactDto> GetFactDtos()
    {
        return new List<FactDto>
        {
            new (Id: 1,
                 Number: 1,
                 Title: "Title 1",
                 FactContent: "Fact content 1",
                 ImageId: 1,
                 StreetcodeId: 1),

            new (Id: 2,
                 Number: 2,
                 Title: "Title 2",
                 FactContent: "Fact content 2",
                 ImageId: 2,
                 StreetcodeId: 2),
        };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()))
            .Returns(GetFactDtos());
    }

    private void MockRepositorySetupReturnsData(int streetcodeId)
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>,
            IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(GetFactsWithMatchingStreetcodeId(streetcodeId));
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>,
            IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync((IEnumerable<Fact>?)null);
    }
}