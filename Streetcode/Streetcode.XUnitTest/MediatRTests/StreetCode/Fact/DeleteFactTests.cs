namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact;

using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.Resources.Errors;

public class DeleteFactTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;

    public DeleteFactTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_DeletesFactAndReturnsOkResult_IfFactFound(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);
        MockHttpContextSetupWithExistingFactId(id);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockHttpContext.Object, _mockLogger.Object, _mockMapper.Object);

        // Act
        var result = await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_RepositoryShouldCallSaveChangesAsyncOnlyOnce_IfFactExists(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);
        MockHttpContextSetupWithExistingFactId(id);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockHttpContext.Object, _mockLogger.Object, _mockMapper.Object);

        // Act
        await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_FactRepositoryShouldCallDeleteOnlyOnce_IfFactExists(int id)
    {
        // Arrange
        MockRepositoryWrapperSetupWithExistingFactId(id);
        MockHttpContextSetupWithExistingFactId(id);

        var handler = new DeleteFactHandler(_mockRepositoryWrapper.Object, _mockHttpContext.Object, _mockLogger.Object, _mockMapper.Object);

        // Act
        await handler.Handle(new DeleteFactCommand(id), CancellationToken.None);

        // Assert
        _mockRepositoryWrapper.Verify(
            repo =>
            repo.FactRepository.Delete(It.IsAny<Fact>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task Handle_ShouldReturnExceptionEntityByIdNotFound_IfFactNotExists(int id)
    {
        // Arrange
        MockHttpContextSetupReturnsNull(id);

        var handler = new GetFactByIdHandler(
            _mockMapper.Object,
            _mockHttpContext.Object,
            _mockLogger.Object);

        var expectedErrorMessage = string.Format(
            ErrorMessages.IncorrectEntity,
            nameof(Fact),
            id);

        // Act
        var result = await handler.Handle(new GetFactByIdQuery(id), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    private static Fact GetFact(int id)
    {
        return new Fact
        {
            Id = id,
        };
    }

    private void MockHttpContextSetupReturnsNull(int id)
    {
        _mockHttpContext.Setup(x => x.HttpContext!.Items["entity"]).Returns(null);
    }

    private void MockHttpContextSetupWithExistingFactId(int id)
    {
        _mockHttpContext.Setup(x => x.HttpContext!.Items["entity"]).Returns(GetFact(id));
    }

    private void MockRepositoryWrapperSetupWithExistingFactId(int id)
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .Delete(GetFact(id)));
        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
    }
}