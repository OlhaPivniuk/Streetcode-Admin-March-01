using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.Dto.Sources;

namespace Streetcode.XUnitTest.ValidationTests.Sources;

public class CreateCategoryRequestDtoValidatorTests
{
    private const int MAXTITLELENGTH = 100;
    private const int MAXTEXTLENGTH = 4000;
    private const int MINIMAGEID = 1;
    private const int MINSTREETCODEID = 1;

    private readonly CreateCategoryRequestDtoValidator _validator;

    public CreateCategoryRequestDtoValidatorTests()
    {
        _validator = new CreateCategoryRequestDtoValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenDtoWithoutTitle()
    {
        // Arrange
        var dto = new CreateCategoryRequestDto(
            Title: "",
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Text: "text");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(MAXTITLELENGTH + 1)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
    {
        // Arrange
        var title = string.Concat(Enumerable.Repeat('a', length));
        var dto = new CreateCategoryRequestDto(
            Title: title,
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Text: "text");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoWithoutText()
    {
        var dto = new CreateCategoryRequestDto(
            Title: "title",
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Text: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [InlineData(MAXTEXTLENGTH + 1)]
    [InlineData(MAXTEXTLENGTH + 10000)]
    public void ShouldHaveError_WhenTextIsLongerThanAllowed(int length)
    {
        // Arrange
        var text = string.Concat(Enumerable.Repeat('a', length));
        var dto = new CreateCategoryRequestDto(
                Title: "title",
                ImageId: MINIMAGEID,
                StreetcodeId: MINSTREETCODEID,
                Text: text);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINIMAGEID - 10000)]
    public void ShouldHaveError_WhenImageIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new CreateCategoryRequestDto(
                    Title: "title",
                    ImageId: id,
                    StreetcodeId: MINSTREETCODEID,
                    Text: "text");

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
        var dto = new CreateCategoryRequestDto(
                    Title: "title",
                    ImageId: MINIMAGEID,
                    StreetcodeId: id,
                    Text: "text");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateCategoryRequestDto(
            Title: "title",
            ImageId: MINIMAGEID,
            StreetcodeId: MINSTREETCODEID,
            Text: "text");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ImageId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Text);
    }
}
