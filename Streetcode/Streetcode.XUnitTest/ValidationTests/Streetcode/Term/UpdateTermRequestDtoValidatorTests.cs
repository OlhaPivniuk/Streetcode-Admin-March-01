using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.Update;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Term;

public class UpdateTermRequestDtoValidatorTests
{
    private const int MINTERMID = 1;
    private const int MINTITLELENGTH = 1;
    private const int MINDESCRIPTIONLENGTH = 1;
    private const int MAXTITLELENGTH = 50;
    private const int MAXDESCRIPTIONLENGTH = 500;

    private readonly UpdateTermRequestDtoValidator _validator;

    public UpdateTermRequestDtoValidatorTests()
    {
        _validator = new UpdateTermRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINTERMID - 10000)]
    public void ShouldHaveError_WhenIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new UpdateTermRequestDto(
            Id: id,
            Title: new string('a', MINTITLELENGTH),
            Description: new string('a', MINDESCRIPTIONLENGTH));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void ShouldHaveError_WhenTitleLengthIsGreaterThanAllowedOrEqualZero(int number)
    {
        // Arrange
        var dto = new UpdateTermRequestDto(
            Id: MINTERMID,
            Title: new string('a', number),
            Description: new string('a', MINDESCRIPTIONLENGTH));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXDESCRIPTIONLENGTH + 10000)]
    public void ShouldHaveError_WhenDescriptionLengthIsGreaterThanAllowedOrEqualZero(int number)
    {
        // Arrange
        var dto = new UpdateTermRequestDto(
            Id: MINTERMID,
            Title: new string('a', MINTITLELENGTH),
            Description: new string('a', number));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new UpdateTermRequestDto(
            Id: MINTERMID,
            Title: new string('a', MINTITLELENGTH),
            Description: new string('a', MINDESCRIPTIONLENGTH));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
