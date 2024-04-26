using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Partners;

public class CreatePartnerSourceLinkRequestDtoValidatorTests
{
    private const int MAXLOGOTYPEENUM = 3;
    private const int MAXTARGETURL = 225;

    private readonly CreatePartnerSourceLinkRequestDtoValidator _validator;

    public CreatePartnerSourceLinkRequestDtoValidatorTests()
    {
        _validator = new CreatePartnerSourceLinkRequestDtoValidator();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(MAXLOGOTYPEENUM + 10000)]
    public void ShouldHaveError_WhenLogoTypeIsNotInLogoTypeEnum(int number)
    {
        // Arrange
        var dto = new CreatePartnerSourceLinkRequestDto(
                    LogoType: (LogoType)number,
                    TargetUrl: null);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.LogoType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXTARGETURL + 10000)]
    public void ShouldHaveError_WhenTargetUrlLengthIsGreaterThanAllowed(int number)
    {
        // Arrange
        var dto = new CreatePartnerSourceLinkRequestDto(
                    LogoType: LogoType.Twitter,
                    TargetUrl: new string('a', number));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.TargetUrl);
    }

    [Fact]
    public void ShouldHaveError_WhenTargetUrlNotContainLogoTypyName()
    {
        // Arrange
        var dto = new CreatePartnerSourceLinkRequestDto(
                    LogoType: LogoType.Twitter,
                    TargetUrl: new string('a', MAXTARGETURL));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.TargetUrl);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreatePartnerSourceLinkRequestDto(
                    LogoType: LogoType.Twitter,
                    TargetUrl: "my.twitter.com");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.LogoType);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.TargetUrl);
    }
}