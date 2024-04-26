namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Term;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Streetcode.TextContent;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class GetAllTermsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetAllTermsHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAllTermsHandler_ShouldReturnOk_IfTermsExist()
    {
        // Arrange
        MockRepositorySetupReturnsTerms();
        MockMapperSetup();

        var handler = new GetAllTermsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllTermsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllTermsHandler_MapperShouldCallMapOnce()
    {
        // Arrange
        MockRepositorySetupReturnsTerms();
        MockMapperSetup();

        var handler = new GetAllTermsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllTermsQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<IEnumerable<TermDto>>(It.IsAny<IEnumerable<Term>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllTermsHandler_ShouldReturnTermDtoCollection_IfTermsExist()
    {
        // Arrange
        MockRepositorySetupReturnsTerms();
        MockMapperSetup();

        var handler = new GetAllTermsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllTermsQuery(), CancellationToken.None);

        // Assert
        Assert.IsAssignableFrom<IEnumerable<TermDto>>(result.Value);
    }

    [Fact]
    public async Task GetAllTermsHandler_ShouldReturnFail_WhenTermsAreNotFound()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllTermsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllTermsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetAllTermsHandler_ShouldLogCorrectErrorMessage_WhenTermsAreNotFound()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllTermsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedMessage = string.Format(
            ErrorMessages.EntitiesNotFound,
            nameof(Term));

        // Act
        var result = await handler.Handle(new GetAllTermsQuery(), CancellationToken.None);
        var actualMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedMessage, actualMessage);
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<TermDto>>(It.IsAny<IEnumerable<Term>>()))
            .Returns(new List<TermDto>
            {
                    new TermDto
                    {
                        Id = 1,
                        Title = "Term 1",
                        Description = "Description 1"
                    },
                    new TermDto
                    {
                        Id = 2,
                        Title = "Term 2",
                        Description = "Description 2"
                    }
            });
    }

    private void MockRepositorySetupReturnsTerms()
    {
        _mockRepositoryWrapper.Setup(x => x.TermRepository
            .GetAllAsync(
            It.IsAny<Expression<Func<Term, bool>>>(),
            It.IsAny<Func<IQueryable<Term>,
        IIncludableQueryable<Term, object>>>()))
            .ReturnsAsync(new List<Term>
            {
                    new Term
                    {
                        Id = 1,
                        Title = "Term 1",
                        Description = "Description 1"
                    },
                    new Term
                    {
                        Id = 2,
                        Title = "Term 2",
                        Description = "Description 2"
                    }
            });
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.TermRepository
            .GetAllAsync(
           It.IsAny<Expression<Func<Term, bool>>>(),
           It.IsAny<Func<IQueryable<Term>,
       IIncludableQueryable<Term, object>>>()))
            .ReturnsAsync((IEnumerable<Term>?)null);
    }
}
