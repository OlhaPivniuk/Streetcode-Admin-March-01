using FluentValidation.TestHelper;
using Streetcode.BLL.Dto.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Media.Art;

public class CreateArtRequestDtoValidatorTests
{
    private const int MAXTITLELENGTH = 150;
    private const int MAXDESCRPTIONLENGTH = 400;
    private const int MINIMAGEID = 1;
    private const int MINSTREETCODEID = 1;

    private readonly CreateArtRequestDtoValidator _validator;

    public CreateArtRequestDtoValidatorTests()
    {
        _validator = new CreateArtRequestDtoValidator();
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoWithoutTitle()
    {
        // Arrange
        var dto = new CreateArtRequestDto(
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
    public void ShouldNotHaveError_WhenDtoWithoutDescription()
    {
        // Arrange
        var dto = new CreateArtRequestDto(
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
    public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
    {
        // Arrange
        var title = string.Concat(Enumerable.Repeat('a', length));
        var dto = new CreateArtRequestDto(
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
    public void ShouldHaveError_WhenDescriptionIsLongerThanAllowed(int length)
    {
        // Arrange
        var description = string.Concat(Enumerable.Repeat('a', length));
        var dto = new CreateArtRequestDto(
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
    public void ShouldHaveError_WhenImageIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new CreateArtRequestDto(
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
    public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new CreateArtRequestDto(
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
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateArtRequestDto(
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