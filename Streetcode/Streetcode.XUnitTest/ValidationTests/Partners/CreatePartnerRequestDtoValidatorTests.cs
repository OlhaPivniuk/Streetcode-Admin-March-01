using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.MediatR.Partners.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Partners;

public class CreatePartnerRequestDtoValidatorTests
{
    private const int MINLOGOID = 1;
    private const int MINTITLELENGTH = 1;
    private const int EXISTEDID = 1;
    private const int MAXTITLELENGTH = 100;
    private const int MAXTARGETURLLENGTH = 200;
    private const int MAXDESCRIPTIONLENGTH = 450;
    private const int URLTITLELENGTH = 1;

    private readonly CreatePartnerRequestDtoValidator _validator;

    public CreatePartnerRequestDtoValidatorTests()
    {
        _validator = new CreatePartnerRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINLOGOID - 10000)]
    public void ShouldHaveError_WhenLogoIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: id,
                    Title: new string('a', MINTITLELENGTH),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: new string('a', MAXTARGETURLLENGTH),
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', MAXDESCRIPTIONLENGTH),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.LogoId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void ShouldHaveError_WhenTitleIsGreaterThanAllowedOrZero(int number)
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: EXISTEDID,
                    Title: new string('a', number),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: new string('a', MAXTARGETURLLENGTH),
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', MAXDESCRIPTIONLENGTH),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(MAXTARGETURLLENGTH + 10000)]
    public void ShouldHaveError_WhenTargetUrlIsGreaterThanAllowed(int number)
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: EXISTEDID,
                    Title: new string('a', MINTITLELENGTH),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: new string('a', number),
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', MAXDESCRIPTIONLENGTH),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.TargetUrl);
    }

    [Fact]
    public void ShouldHaveError_WhenTargetUrlIsNullButUrlTitleIsNotNull()
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: EXISTEDID,
                    Title: new string('a', MINTITLELENGTH),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: null,
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', MAXDESCRIPTIONLENGTH),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.UrlTitle);
    }

    [Fact]
    public void ShouldHaveError_WhenTargetUrlIsEmptyButUrlTitleIsNotNull()
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: EXISTEDID,
                    Title: new string('a', MINTITLELENGTH),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: string.Empty,
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', MAXDESCRIPTIONLENGTH),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.UrlTitle);
    }

    [Theory]
    [InlineData(MAXDESCRIPTIONLENGTH + 10000)]
    public void ShouldHaveError_WhenDescriptionIsGreaterThanAllowed(int number)
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: EXISTEDID,
                    Title: new string('a', MINTITLELENGTH),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: new string('a', MAXTARGETURLLENGTH),
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', number),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreatePartnerRequestDto(
                    LogoId: EXISTEDID,
                    Title: new string('a', MINTITLELENGTH),
                    IsKeyPartner: default,
                    IsVisibleEverywhere: default,
                    TargetUrl: new string('a', MAXTARGETURLLENGTH),
                    UrlTitle: new string('a', URLTITLELENGTH),
                    Description: new string('a', MAXDESCRIPTIONLENGTH),
                    Streetcodes: new List<int>(),
                    PartnerSourceLinks: new List<CreatePartnerSourceLinkRequestDto>());

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.LogoId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.TargetUrl);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.UrlTitle);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}