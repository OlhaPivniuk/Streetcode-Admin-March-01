namespace Streetcode.XUnitTest.ActionFilters;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.ActionFilters;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Contracts;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

public class AsyncValidateEntityExistsAttributeTests
{
    private readonly Mock<IEntityRepositoryBase<IEntity>> _mockRepositoryBase;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;

    public AsyncValidateEntityExistsAttributeTests()
    {
        _mockRepositoryBase = new Mock<IEntityRepositoryBase<IEntity>>();
        _mockLogger = new Mock<ILoggerService>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvokeWithRequestDoesNotContainIdParameter_ReturnsBadRequestObjectResult()
    {
        // Arrange
        var validateUntityExistsActionFilter =
            new AsyncValidateEntityExistsAttribute<IEntity>(_mockRepositoryBase.Object, _mockLogger.Object, _mockMapper.Object);
        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(httpContext, new(), new(), new());
        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: null);

        // Act
        await validateUntityExistsActionFilter.OnActionExecutionAsync(actionExecutingContext, null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvokeWithContextActionArgumentsContainKeyid_CallGetFirstOrDefaultAsync()
    {
        // Arrange
        _mockRepositoryBase.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<IEntity, bool>>>(),
                It.IsAny<Func<IQueryable<IEntity>, IIncludableQueryable<Fact, object>>>()))
                .ReturnsAsync(It.IsAny<IEntity>);

        var validateUntityExistsActionFilter =
            new AsyncValidateEntityExistsAttribute<IEntity>(_mockRepositoryBase.Object, _mockLogger.Object, _mockMapper.Object);
        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(httpContext, new(), new(), new());
        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>() { { "id", 1 } },
            controller: null);

        // Act
        await validateUntityExistsActionFilter.OnActionExecutionAsync(actionExecutingContext, null);

        // Assert
        _mockRepositoryBase.Verify(
            repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<IEntity, bool>>>(),
                It.IsAny<Func<IQueryable<IEntity>, IIncludableQueryable<IEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnActionExecutionAsync_InvokeWithEntityIsEqualNull_ReturnsNotFoundObjectResult()
    {
        // Arrange
        _mockRepositoryBase.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<IEntity, bool>>>(),
                It.IsAny<Func<IQueryable<IEntity>, IIncludableQueryable<Fact, object>>>()))
                .ReturnsAsync((IEntity)null);

        var validateUntityExistsActionFilter =
            new AsyncValidateEntityExistsAttribute<IEntity>(_mockRepositoryBase.Object, _mockLogger.Object, _mockMapper.Object);
        var httpContext = new DefaultHttpContext();

        var actionContext = new ActionContext(httpContext, new(), new(), new());
        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>() { { "id", 1 } },
            controller: null);

        // Act
        await validateUntityExistsActionFilter.OnActionExecutionAsync(actionExecutingContext, null);

        // Assert
        Assert.IsType<NotFoundObjectResult>(actionExecutingContext.Result);
    }
}
