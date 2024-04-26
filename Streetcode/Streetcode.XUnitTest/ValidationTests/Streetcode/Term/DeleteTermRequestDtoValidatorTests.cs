using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Term;

public class DeleteTermRequestDtoValidatorTests
{
    private const int MINID = 1;

    private readonly DeleteTermRequestDtoValidator _validator;

    public DeleteTermRequestDtoValidatorTests()
    {
        _validator = new DeleteTermRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINID - 10000)]
    public void ShouldHaveError_WhenIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new DeleteTermRequestDto(Id: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new DeleteTermRequestDto(Id: MINID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
