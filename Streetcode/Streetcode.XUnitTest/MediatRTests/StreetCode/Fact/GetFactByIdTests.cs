namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact;

using AutoMapper;
using Moq;
using Xunit;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Microsoft.AspNetCore.Http;

public class GetFactByIdTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHttpContextAccessor> _mockHttpContext;

    public GetFactByIdTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockHttpContext = new Mock<IHttpContextAccessor>();
    }

    [Theory]
    [InlineData(1)]
    public async Task GetFactById_ShouldReturnOk_IfIdExists(int id)
    {
        // Arrange
        MockHttpContextSetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetFactByIdHandler(
            _mockMapper.Object,
            _mockHttpContext.Object);

        // Act
        var result = await handler.Handle(new GetFactByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetFactById_RepositoryShouldCallHttpContextItemsOnlyOnce_IfFactExists(int id)
    {
        // Arrange
        MockHttpContextSetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetFactByIdHandler(
            _mockMapper.Object,
            _mockHttpContext.Object);

        // Act
        await handler.Handle(new GetFactByIdQuery(id), CancellationToken.None);

        // Assert
        _mockHttpContext.Verify(
            x =>
            x.HttpContext!.Items["entity"], Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetFactById_MapperShouldCallMapOnlyOnce_IfFactExists(int id)
    {
        // Arrange
        MockHttpContextSetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetFactByIdHandler(
            _mockMapper.Object,
            _mockHttpContext.Object);

        // Act
        await handler.Handle(new GetFactByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper => mapper.Map<FactDto>(It.IsAny<Fact>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetFactById_ShouldReturnFactWithCorrectId_IfFactExists(int id)
    {
        // Arrange
        MockHttpContextSetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetFactByIdHandler(
            _mockMapper.Object,
            _mockHttpContext.Object);

        // Act
        var result = await handler.Handle(new GetFactByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.Equal(id, result.Value.Id);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetFactById_ShouldReturnFactDto_IfFactExists(int id)
    {
        // Arrange
        MockHttpContextSetupReturnsFact(id);
        MockMapperSetup(id);

        var handler = new GetFactByIdHandler(
            _mockMapper.Object,
            _mockHttpContext.Object);

        // Act
        var result = await handler.Handle(new GetFactByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.IsType<FactDto>(result.Value);
    }

    private void MockHttpContextSetupReturnsFact(int id)
    {
        _mockHttpContext.Setup(x => x.HttpContext!.Items["entity"]).Returns(new Fact { Id = id });
    }

    private void MockHttpContextSetupReturnsNull(int id)
    {
        _mockHttpContext.Setup(x => x.HttpContext!.Items["entity"]).Returns(null);
    }

    private void MockMapperSetup(int id)
    {
        _mockMapper.Setup(x => x
            .Map<FactDto>(It.IsAny<Fact>()))
            .Returns(new FactDto(
                Id: id,
                Number: 1,
                Title: "Title 1",
                FactContent: "Fact content 1",
                ImageId: 1,
                StreetcodeId: 1));
    }
}
