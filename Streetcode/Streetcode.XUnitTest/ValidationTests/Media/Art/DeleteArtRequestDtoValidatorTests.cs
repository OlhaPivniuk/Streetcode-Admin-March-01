using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Media.Art;

public class DeleteArtRequestDtoValidatorTests
{
    private const int MINARTID = 1;

    private readonly DeleteArtRequestDtoValidator _validator;

    public DeleteArtRequestDtoValidatorTests()
    {
        _validator = new DeleteArtRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINARTID - 10000)]
    public void ShouldHaveError_WhenArtIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new DeleteArtRequestDto(Id: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new DeleteArtRequestDto(Id: MINARTID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
