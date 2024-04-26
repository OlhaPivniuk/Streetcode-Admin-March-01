using System.Linq.Expressions;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.DTO.Partners.Delete;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners;

public class DeletePartnerHandlerTests
{
    private const int MINTITLELENGTH = 1;
    private const int EXISTEDID = 1;
    private const int MAXTARGETURLLENGTH = 200;
    private const int MAXDESCRIPTIONLENGTH = 450;
    private const int URLTITLELENGTH = 1;
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeletePartnerHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidDeletePartnerRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeletePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeletePartnerRequest();
        var expectedType = typeof(Result<DeletePartnerResponseDto>);
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeletePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeletePartnerRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeletePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnErrorDeleteFailed_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidDeletePartnerRequest();
        SetupMock(FAILEDSAVE);
        var handler = DeleteHandler();
        var command = new DeletePartnerCommand(request);

        var expectedErrorMessage = string.Format(
        ErrorMessages.DeleteFailed,
        typeof(Partner).Name,
        request.Id);

        // Act
        var result = await handler.Handle(command, _cancellationToken);
        var actualErrorMessage = result.Errors[0].Message;

        // Assert
        Assert.Equal(expectedErrorMessage, actualErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncOnce_IfInputIsValid()
    {
        // Arrange
        var request = GetValidDeletePartnerRequest();
        SetupMock(SUCCESSFULSAVE);
        var handler = DeleteHandler();
        var command = new DeletePartnerCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(1));
    }

    private DeletePartnerHandler DeleteHandler()
    {
        return new DeletePartnerHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(int saveChangesAsyncResult)
    {
        var partner = new Partner
        {
            Id = EXISTEDID,
            Title = new string('a', MINTITLELENGTH),
            LogoId = EXISTEDID,
            IsKeyPartner = default,
            IsVisibleEverywhere = default,
            TargetUrl = new string('a', MAXTARGETURLLENGTH),
            UrlTitle = new string('a', URLTITLELENGTH),
            Logo = new Image
            {
                Id = EXISTEDID,
                MimeType = "image/gif",
                Streetcodes = new List<StreetcodeContent>()
            },
            Description = new string('a', MAXDESCRIPTIONLENGTH),
            Streetcodes = new List<StreetcodeContent>()
        };

        _mockRepositoryWrapper
            .Setup(repo => repo.PartnersRepository.GetFirstOrDefaultAsync(
                AnyEntityPredicate<Partner>(),
                AnyEntityInclude<Partner>()))
            .ReturnsAsync(partner);

        _mockRepositoryWrapper
            .Setup(repo => repo.PartnersRepository.Delete(partner));

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static DeletePartnerRequestDto GetValidDeletePartnerRequest()
    {
        return new(Id: 1);
    }
}