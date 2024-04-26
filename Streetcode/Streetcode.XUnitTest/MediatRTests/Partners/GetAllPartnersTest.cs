namespace Streetcode.XUnitTest.MediatRTests.Partners;

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.BLL.Dto.Partners;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.DAL.Entities.Partners;
using Xunit;

public class GetAllPartnersTest
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public GetAllPartnersTest()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task GetAllPartners_ShouldReturnOk_WhenPartnerExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllPartnersHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetAllPartners_ShouldReturnCollectionOfCorrectCount_WhenPartnersExist()
    {
        // Arrange
        var mockFacts = GetPartnerList();
        var expectedCount = mockFacts.Count;

        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllPartnersHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        var result = await handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);
        var actualCount = result.Value.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public async Task GetAllPartners_ShouldLogCorrectErrorMessage_WhenPartnersAreNull()
    {
        // Arrange
        MockRepositorySetupReturnsNull();

        var handler = new GetAllPartnersHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        var expectedError = "Cannot find any partners";

        // Act
        var result = await handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(expectedError, result.Errors[0].Message);
    }

    [Fact]
    public async Task GetAllPartners_MapperShouldMapOnlyOnce_WhenFactsExist()
    {
        // Arrange
        MockRepositorySetupReturnsData();
        MockMapperSetup();

        var handler = new GetAllPartnersHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);

        // Act
        await handler.Handle(new GetAllPartnersQuery(), CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            mapper =>
            mapper.Map<IEnumerable<PartnerDto>>(It.IsAny<IEnumerable<Partner>>()), Times.Once);
    }

    private static List<Partner> GetPartnerList()
    {
        return new List<Partner>
{
    new ()
    {
        Id = 1
    },
    new ()
    {
        Id = 2
    },
    new ()
    {
        Id = 3
    },
};
    }

    private static List<PartnerDto> GetPartnerDtoList()
    {
        return new List<PartnerDto>
    {
        new ()
        {
            Id = 1
        },

        new ()
        {
            Id = 2
        },
        new ()
        {
            Id = 3
        },
    };
    }

    private void MockMapperSetup()
    {
        _mockMapper.Setup(x => x
            .Map<IEnumerable<PartnerDto>>(It.IsAny<IEnumerable<Partner>>()))
            .Returns(GetPartnerDtoList());
    }

    private void MockRepositorySetupReturnsData()
    {
        _mockRepositoryWrapper.Setup(x => x.PartnersRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
            IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync(GetPartnerList());
    }

    private void MockRepositorySetupReturnsNull()
    {
        _mockRepositoryWrapper.Setup(x => x.PartnersRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Partner, bool>>>(),
                It.IsAny<Func<IQueryable<Partner>,
            IIncludableQueryable<Partner, object>>>()))
            .ReturnsAsync((IEnumerable<Partner>?)null);
    }
}
