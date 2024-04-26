namespace Streetcode.XUnitTest.MediatRTests.StreetCode.Fact;

using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Linq.Expressions;
using Xunit;
using ReorderFactErrors = BLL.Resources.Errors.ValidationErrors.Fact.ReorderFactErrors;

public class ReorderFactHandlerTests
{
    private const int STREETCODEID = 1;
    private const int FAILEDSTREETCODEID = 10;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    public ReorderFactHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Theory]
    [InlineData(new int[] { 18, 2, 1 })]
    public async Task Handle_WithCorrectData_ShouldReturnSucceed(int[] ids)
    {
        // Arrange
        MockRepositorySetupReturnsSuccessResult();
        var handler = new ReorderFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new ReorderFactCommand(new ReorderFactRequestDto(ids, STREETCODEID)), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData(new int[] { })]
    public async Task Handle_WithNullOrEmptyArrOffIds_ShouldReturnError_IncomingFactIdArrIsNullOrEmpty(int[] ids)
    {
        // Arrange
        MockRepositorySetupNullOrEmptyArrOffIds();
        var handler = new ReorderFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new ReorderFactCommand(new ReorderFactRequestDto(ids, STREETCODEID)), CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == ReorderFactErrors.IncomingFactIdArrIsNullOrEmpty);
    }

    [Theory]
    [InlineData(new int[] { 18, 2, 1 })]
    public async Task Handle_WithFailedStreetcodeId_ShouldReturnError_ThereAreNoFactsWithCorrespondingStreetcodeId(int[] ids)
    {
        // Arrange
        MockRepositorySetupWithFailedStreetcodeId();
        var handler = new ReorderFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new ReorderFactCommand(new ReorderFactRequestDto(ids, FAILEDSTREETCODEID)), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;
        var expectedErrorMessage = string.Format(
           ReorderFactErrors.ThereAreNoFactsWithCorrespondingStreetcodeId,
           FAILEDSTREETCODEID);

        // Assert
        result.IsFailed.Should().BeTrue();
        Assert.Equal(expectedErrorMessage, actualErrorMessage);

        // redundant
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Theory]
    [InlineData(new int[] { 2, 1 })]
    public async Task Handle_WithIncorrectIdsNumberInArray_ShouldReturnError_IncorrectIdsNumberInArray(int[] ids)
    {
        // Arrange
        MockRepositorySetupWithIncorrectIdsNumberInArray();
        var handler = new ReorderFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new ReorderFactCommand(new ReorderFactRequestDto(ids, STREETCODEID)), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;
        var expectedErrorMessage = string.Format(
           ReorderFactErrors.IncorrectIdsNumberInArray,
           ids.Length,
           GetFactList().Count,
           STREETCODEID);

        // Assert
        result.IsFailed.Should().BeTrue();
        Assert.Equal(expectedErrorMessage, actualErrorMessage);

        // redundant
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Theory]
    [InlineData(new int[] { 2, 1, 100 })]
    public async Task Handle_WithIncorrectIdsNumberInArray_ShouldReturnError_IncorrectFactIdInArray(int[] ids)
    {
        // Arrange
        MockRepositorySetupWithIncorrectFactIdInArray();
        var handler = new ReorderFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new ReorderFactCommand(new ReorderFactRequestDto(ids, STREETCODEID)), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;
        var expectedErrorMessage = string.Format(
           ReorderFactErrors.IncorrectFactIdInArray,
           ids[0],
           STREETCODEID);

        // Assert
        result.IsFailed.Should().BeTrue();
        Assert.Equal(expectedErrorMessage, actualErrorMessage);

        // redundant
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Theory]
    [InlineData(new int[] { 2, 1, 18 })]
    public async Task Handle_WithIncorrectIdsNumberInArray_ShouldReturnError_CannotUpdateNumberInFact(int[] ids)
    {
        // Arrange
        MockRepositorySetupCannotUpdateNumberInFact();
        var handler = new ReorderFactHandler(_mockRepositoryWrapper.Object, _mockLogger.Object);

        // Act
        var result = await handler.Handle(new ReorderFactCommand(new ReorderFactRequestDto(ids, STREETCODEID)), CancellationToken.None);
        var actualErrorMessage = result.Errors[0].Message;
        var expectedErrorMessage = string.Format(ReorderFactErrors.CannotUpdateNumberInFact);

        // Assert
        result.IsFailed.Should().BeTrue();
        Assert.Equal(expectedErrorMessage, actualErrorMessage);

        // redundant
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    private void MockRepositorySetupReturnsSuccessResult()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(GetFactList());

        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(new Fact());

        _mockRepositoryWrapper
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
    }

    private void MockRepositorySetupNullOrEmptyArrOffIds()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync((IEnumerable<Fact>?)null);
    }

    private void MockRepositorySetupWithFailedStreetcodeId()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
             .ReturnsAsync(GetFactList().Where(f => f.StreetcodeId == FAILEDSTREETCODEID));
    }

    private void MockRepositorySetupWithIncorrectIdsNumberInArray()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
             .ReturnsAsync(GetFactList());
    }

    private void MockRepositorySetupWithIncorrectFactIdInArray()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
             .ReturnsAsync(GetFactList());

        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
             .ReturnsAsync(GetFactList().FirstOrDefault(f => f.Id == 100));
    }

    private void MockRepositorySetupCannotUpdateNumberInFact()
    {
        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetAllAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(GetFactList());

        _mockRepositoryWrapper.Setup(x => x.FactRepository
            .GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Fact, bool>>>(),
                It.IsAny<Func<IQueryable<Fact>, IIncludableQueryable<Fact, object>>>()))
            .ReturnsAsync(new Fact());

        _mockRepositoryWrapper
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(0);
    }

    private static List<Fact> GetFactList()
    {
        return new List<Fact>
        {
            new ()
            {
                Id = 1,
                Number = 2,
                Title = "Title1",
                FactContent = "Fact content 1",
                ImageId = 1,
                StreetcodeId = 1
            },
            new ()
            {
                Id = 2,
                Number = 1,
                Title = "Title2",
                FactContent = "Fact content 2",
                ImageId = 2,
                StreetcodeId = 1
            },
            new ()
            {
                Id = 18,
                Number = 3,
                Title = "Title3",
                FactContent = "Fact content 18",
                ImageId = 3,
                StreetcodeId = 1
            },
        };
    }
}