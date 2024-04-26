using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Resources.Errors;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Partners;

public class CreatePartnerHandlerTests
{
    private const int MINTITLELENGTH = 1;
    private const int EXISTEDID = 1;
    private const int MAXTARGETURLLENGTH = 200;
    private const int MAXDESCRIPTIONLENGTH = 450;
    private const int URLTITLELENGTH = 1;
    private const int SUCCESSFULSAVE = 1;
    private const int FAILEDSAVE = -1;

    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILoggerService> _mockLogger;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreatePartnerHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnOkResult_IfCommandHasValidInput()
    {
        // Arrange
        var request = GetValidCreatePartnerRequestDto();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreatePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNonexistentLogoId()
    {
        // Arrange
        var request = new CreatePartnerRequestDto(
                   LogoId: int.MaxValue,
                   Title: new string('a', MINTITLELENGTH),
                   IsKeyPartner: default,
                   IsVisibleEverywhere: default,
                   TargetUrl: new string('a', MAXTARGETURLLENGTH),
                   UrlTitle: new string('a', URLTITLELENGTH),
                   Description: new string('a', MAXDESCRIPTIONLENGTH),
                   Streetcodes: new List<int>(),
                   PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());
        var expectedErrorMessage = "Invalid image file. Please upload an gif, jpeg or png file.";

        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreatePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnSingleErrorWithCorrectMessage_IfCommandWithNotUniqueLogoId()
    {
        // Arrange
        var request = new CreatePartnerRequestDto(
                   LogoId: int.MaxValue,
                   Title: new string('a', MINTITLELENGTH),
                   IsKeyPartner: default,
                   IsVisibleEverywhere: default,
                   TargetUrl: new string('a', MAXTARGETURLLENGTH),
                   UrlTitle: new string('a', URLTITLELENGTH),
                   Description: new string('a', MAXDESCRIPTIONLENGTH),
                   Streetcodes: new List<int>(),
                   PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        var expectedErrorMessage = string.Format(
            ErrorMessages.PotencialPrimaryKeyIsNotUnique,
            "Logo",
            request.LogoId);

        SetupMock(request, SUCCESSFULSAVE, false);
        var handler = CreateHandler();
        var command = new CreatePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Errors.Should().ContainSingle(e => e.Message == expectedErrorMessage);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultOfCorrectType_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreatePartnerRequestDto();
        var expectedType = typeof(Result<CreatePartnerResponseDto>);
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreatePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeOfType(expectedType);
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFail_IfSavingOperationFailed()
    {
        // Arrange
        var request = GetValidCreatePartnerRequestDto();
        SetupMock(request, FAILEDSAVE);

        var handler = CreateHandler();
        var command = new CreatePartnerCommand(request);

        // Act
        var result = await handler.Handle(command, _cancellationToken);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsyncTwice_IfInputIsValid()
    {
        // Arrange
        var request = GetValidCreatePartnerRequestDto();
        SetupMock(request, SUCCESSFULSAVE);
        var handler = CreateHandler();
        var command = new CreatePartnerCommand(request);

        // Act
        await handler.Handle(command, _cancellationToken);

        // Assert
        _mockRepositoryWrapper.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
    }

    private CreatePartnerHandler CreateHandler()
    {
        return new CreatePartnerHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }

    private void SetupMock(CreatePartnerRequestDto request, int saveChangesAsyncResult, bool IsLogoUnique = true)
    {
        var logo = request.LogoId switch
        {
            EXISTEDID => new Image
            {
                Id = request.LogoId,
                MimeType = "image/gif",
                Streetcodes = new List<StreetcodeContent>()
            },
            _ => null,
        };

        var partner = GetValidPartner();

        _mockRepositoryWrapper
          .Setup(r => r.PartnersRepository.GetFirstOrDefaultAsync(
              It.IsAny<Expression<Func<Partner, bool>>>(), null))
              .ReturnsAsync(IsLogoUnique ? null : partner);

        _mockRepositoryWrapper.Setup(x => x.BeginTransaction())
               .Returns(new System.Transactions.TransactionScope());

        _mockRepositoryWrapper
          .Setup(r => r.ImageRepository.GetSingleOrDefaultAsync(
              It.IsAny<Expression<Func<Image, bool>>>(), null))
              .ReturnsAsync(logo);

        _mockRepositoryWrapper
            .Setup(r => r.StreetcodeRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null))
                .ReturnsAsync(It.IsAny<List<StreetcodeContent>>());

        _mockRepositoryWrapper.Setup(repo => repo.PartnersRepository.Create(
            It.IsAny<Partner>())).Returns(partner);

        _mockRepositoryWrapper.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(saveChangesAsyncResult);

        _mockMapper
            .Setup(m => m.Map<Partner>(It.IsAny<CreatePartnerRequestDto>())).Returns(partner);
    }

    private static Expression<Func<TEntity, bool>> AnyEntityPredicate<TEntity>()
    {
        return It.IsAny<Expression<Func<TEntity, bool>>>();
    }

    private static Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> AnyEntityInclude<TEntity>()
    {
        return It.IsAny<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>>();
    }

    private static CreatePartnerRequestDto GetValidCreatePartnerRequestDto()
    {
        return new CreatePartnerRequestDto(
                   LogoId: EXISTEDID,
                   Title: new string('a', MINTITLELENGTH),
                   IsKeyPartner: default,
                   IsVisibleEverywhere: default,
                   TargetUrl: new string('a', MAXTARGETURLLENGTH),
                   UrlTitle: new string('a', URLTITLELENGTH),
                   Description: new string('a', MAXDESCRIPTIONLENGTH),
                   Streetcodes: new List<int>(),
                   PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());
    }

    private static Partner GetValidPartner()
    {
        return new Partner
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
    }
}