namespace Streetcode.XUnitTest.MediatRTests.Partners;

using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;
using Streetcode.BLL.Dto.Partners;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;

public class GetPartnerByIdTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetPartnerByIdTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(1)]
    public async Task GetPartnerById_ShouldReturnOk_IfIdExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsPartner();
        MockMapperSetup();

        var handler = new GetPartnerByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetPartnerByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetPartnerById_ShouldLogCorrectErrorMessage_WhenPartnerIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetPartnerByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expected = $"Cannot find any partner with corresponding id: {id}";

        // Act
        var result = await handler.Handle(new GetPartnerByIdQuery(id), CancellationToken.None);
        var actual = result.Errors[0].Message;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetPartnerById_ShouldReturnFail_WhenPartnerIsNotFound(int id)
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetPartnerByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetPartnerByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(1)]
    public async Task GetPartnerById_MapperShouldCallMapOnlyOnce_IfPartnerExists(int id)
    {
        // Arrange
        MockRepositorySetupReturnsPartner();
        MockMapperSetup();

        var handler = new GetPartnerByIdHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetPartnerByIdQuery(id), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            m => m.Map<PartnerDto>(It.IsAny<Partner>()),
            Times.Once);
    }

    private static PartnerDto GetPartnerDto()
    {
        return new PartnerDto
        {
            Id = 1
        };
    }

    private static Partner GetPartner()
    {
        return new Partner
        {
            Id = 1
        };
    }

    private void MockMapperSetup()
    {
        _mockMapper
            .Setup(x => x
            .Map<PartnerDto>(It.IsAny<Partner>()))
            .Returns(GetPartnerDto());
    }

    private void MockRepositorySetupReturnsPartner()
    {
        _mockRepositoryWrapper.Setup(x => x.PartnersRepository
            .GetSingleOrDefaultAsync(
               It.IsAny<Expression<Func<Partner, bool>>>(),
               It.IsAny<Func<IQueryable<Partner>,
                IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(GetPartner);
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.PartnersRepository
            .GetSingleOrDefaultAsync(
               It.IsAny<Expression<Func<Partner, bool>>>(),
               It.IsAny<Func<IQueryable<Partner>,
               IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync((Partner?)null);
    }
}
