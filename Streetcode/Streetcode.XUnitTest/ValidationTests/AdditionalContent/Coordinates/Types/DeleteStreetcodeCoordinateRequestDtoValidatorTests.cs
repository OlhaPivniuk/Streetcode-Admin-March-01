using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.AdditionalContent.Coordinates.Types;

public class DeleteStreetcodeCoordinateRequestDtoValidatorTests
{
    private const int MINID = 1;

    private readonly DeleteStreetcodeCoordinateRequestDtoValidator _validator;

    public DeleteStreetcodeCoordinateRequestDtoValidatorTests()
    {
        _validator = new DeleteStreetcodeCoordinateRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINID - 10000)]
    public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new DeleteStreetcodeCoordinateRequestDto(Id: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new DeleteStreetcodeCoordinateRequestDto(Id: MINID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}