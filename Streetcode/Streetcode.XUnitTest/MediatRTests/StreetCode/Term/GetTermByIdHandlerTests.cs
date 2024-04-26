namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Term;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetTermByIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetTermByIdHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetTermByIdHandler_ShouldReturnOk_IfIdExists()
    {
        // Arrange
        int id = 1;
        MockRepositorySetupReturnsTerm(id);
        MockMapperSetup(id);

        var handler = new GetTermByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTermByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetTermByIdHandler_RepositoryShouldCallGetFirstOrDefaultAsyncOnce_IfTermExists()
    {
        // Arrange
        int id = 1;
        MockRepositorySetupReturnsTerm(id);
        MockMapperSetup(id);

        var handler = new GetTermByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetTermByIdQuery(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo => repo.TermRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Term, bool>>>(),
                It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTermByIdHandler_MapperShouldCallMapOnce_IfTermExists()
    {
        // Arrange
        int id = 1;
        MockRepositorySetupReturnsTerm(id);
        MockMapperSetup(id);

        var handler = new GetTermByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetTermByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<TermDto>(It.IsAny<Term>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTermByIdHandler_ShouldReturnTermDto_IfTermExists()
    {
        // Arrange
        int id = 1;
        MockRepositorySetupReturnsTerm(id);
        MockMapperSetup(id);

        var handler = new GetTermByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTermByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.IsType<TermDto>(result.Value);
    }

    [Fact]
    public async Task GetTermByIdHandler_ShouldReturnFail_WhenTermIsNotFound()
    {
        // Arrange
        int id = 1;
        MockRepositorySetupReturnsNull();

        var handler = new GetTermByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetTermByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetTermByIdHandler_ShouldLogCorrectErrorMessage_WhenTermIsNotFound()
    {
        // Arrange
        int id = 1;
        MockRepositorySetupReturnsNull();

        var handler = new GetTermByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedMessage = string.Format(
            ErrorMessages.EntityByIdNotFound,
            nameof(Term),
            id);

        // Act
        var result = await handler.Handle(new GetTermByIdQuery(id), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<TermDto>(It.IsAny<Term>()))
            .Returns(new TermDto
            {
                Id = id,
                Title = "Test Term",
                Description = "Test Description"
            });
    }

    private void MockRepositorySetupReturnsTerm(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.TermRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Term, bool>>>(),
                It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>>()))
            .ReturnsAsync(new Term
            {
                Id = id,
                Title = "Test Term",
                Description = "Test Description"
            });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.TermRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Term, bool>>>(),
                It.IsAny<Func<IQueryable<Term>, IIncludableQueryable<Term, object>>>()))
            .ReturnsAsync((Term?)null);
    }
}
