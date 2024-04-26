namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact;

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetAllFactsTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetAllFactsTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAllFacts_ShouldReturnOk_WhenFactsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllFacts_ShouldReturnCollectionOfCorrectCount_WhenFactsExist()
    {
        // Arrange
        var mockFacts = GetFactList();
        var expectedCount = mockFacts.Count;

        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async Task GetAllFacts_RepositoryShouldCallGetAllAsyncOnlyOnce_WhenFactsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.FactRepository.GetAllAsync(null, null), Times.Once);
    }

    [Fact]
    public async Task GetAllFacts_MapperShouldMapOnlyOnce_WhenFactsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFacts_ShouldReturnCollectionOfFactDto_WhenFactsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

        // Assert
        Assert.IsType<List<FactDto>>(result.Value);
    }

    [Fact]
    public async Task GetAllFacts_ShouldReturnFail_WhenFactsAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetAllFacts_ShouldLogCorrectErrorMessage_WhenFactsAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllFactsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedError = string.Format(ErrorMessages.EntitiesNotFound, nameof(Fact));

        // Act
        var result = await handler.Handle(new GetAllFactsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    private static List<Fact> GetFactList()
    {
        return new List<Fact>
        {
            new ()
            {
                Id = 1,
                Title = "Title1",
                FactContent = "Fact content 1",
                ImageId = 1,
            },
            new ()
            {
                Id = 2,
                Title = "Title2",
                FactContent = "Fact content 2",
                ImageId = 2,
            },
            new ()
            {
                Id = 3,
                Title = "Title3",
                FactContent = "Fact content 3",
                ImageId = 3,
            },
        };
    }

    private static List<FactDto> GetFactDtoList()
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

            new (Id: 3,
                 Number: 3,
                 Title: "Title 3",
                 FactContent: "Fact content 3",
                 ImageId: 3,
                 StreetcodeId: 3),
        };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<FactDto>>(It.IsAny<IEnumerable<Fact>>()))
            .Returns(GetFactDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>,
            IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(GetFactList());
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
