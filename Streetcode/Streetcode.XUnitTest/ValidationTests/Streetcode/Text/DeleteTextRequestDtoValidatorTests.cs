using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Text;

public class DeleteTextRequestDtoValidatorTests
{
    private const int MINID = 1;

    private readonly DeleteTextRequestDtoValidator _validator;

    public DeleteTextRequestDtoValidatorTests()
    {
        _validator = new DeleteTextRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINID - 10000)]
    public void ShouldHaveError_WhenIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new DeleteTextRequestDto(Id: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new DeleteTextRequestDto(Id: MINID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}