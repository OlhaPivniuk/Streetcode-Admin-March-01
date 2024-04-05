using FluentValidation.TestHelper;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests;

public class CreateArtValidatorTests
{
    private const int MAXTITLELENGTH = 150;
    private const int MAXDESCRPTIONLENGTH = 400;
    private const int MINIMAGEID = 1;
    private const int MINSTREETCODEID = 1;

    private readonly CreateArtDtoValidator _validator;

    public CreateArtValidatorTests()
    {
        _validator = new CreateArtDtoValidator();
    }

    [Fact]
    public void Should_not_have_error_when_dto_without_Title()
    {
        // Arrange
        var dto = new CreateArtDto(
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Title: "",
            Description: "description");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_not_have_error_when_dto_without_Description()
    {
        // Arrange
        var dto = new CreateArtDto(
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Title: "title",
            Description: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(MAXTITLELENGTH + 1)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void Should_have_error_when_Title_is_longerThanAllowed(int length)
    {
        // Arrange
        var title = string.Concat(Enumerable.Repeat('a', length));
        var dto = new CreateArtDto(
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Title: title,
            Description: "description");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(MAXDESCRPTIONLENGTH + 1)]
    [InlineData(MAXDESCRPTIONLENGTH + 10000)]
    public void Should_have_error_when_Description_is_longerThanAllowed(int length)
    {
        // Arrange
        var description = string.Concat(Enumerable.Repeat('a', length));
        var dto = new CreateArtDto(
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Title: "title",
            Description: description);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINIMAGEID - 10000)]
    public void Should_have_error_when_ImageId_is_zero_or_negative(int id)
    {
        // Arrange
        var dto = new CreateArtDto(
            ImageId: id,
            StreetcodeId: MINSTREETCODEID,
            Title: "title",
            Description: "description");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ImageId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINSTREETCODEID - 10000)]
    public void Should_have_error_when_StreetcodeId_is_zero_or_negative(int id)
    {
        // Arrange
        var dto = new CreateArtDto(
            ImageId: MINIMAGEID,
            StreetcodeId: id,
            Title: "title",
            Description: "description");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Fact]
    public void Should_not_have_error_when_dto_is_valid()
    {
        // Arrange
        var dto = new CreateArtDto(
            MINIMAGEID,
            MINSTREETCODEID,
            "title",
            "description");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ImageId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
