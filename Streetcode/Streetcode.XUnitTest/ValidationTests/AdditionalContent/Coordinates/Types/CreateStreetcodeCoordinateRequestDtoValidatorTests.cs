using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.StreetcodeCoordinate.Create;

namespace Streetcode.XUnitTest.ValidationTests.AdditionalContent.Coordinates.Types;

public class CreateStreetcodeCoordinateRequestDtoValidatorTests
{
    private const int MINSTREETCODEID = 1;
    private const int MINLATITUDE = -90;
    private const int MAXLATITUDE = 90;
    private const int MINLONGTITUDE = -180;
    private const int MAXLONGTITUDE = 180;

    private readonly CreateStreetcodeCoordinateRequestDtoValidator _validator;

    public CreateStreetcodeCoordinateRequestDtoValidatorTests()
    {
        _validator = new CreateStreetcodeCoordinateRequestDtoValidator();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(MINLATITUDE - 10000)]
    [InlineData(MAXLATITUDE + 10000)]
    public void ShouldHaveError_WhenLatitudeIsLessOrGraterThanAllowed(int latitude)
    {
        // Arrange
        var dto = new CreateStreetcodeCoordinateRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    Latitude: latitude,
                    Longtitude: MINLONGTITUDE);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(MINLATITUDE - 10000)]
    [InlineData(MAXLONGTITUDE + 10000)]
    public void ShouldHaveError_WhenLongtitudeIsLessOrGraterThanAllowed(int longtitude)
    {
        // Arrange
        var dto = new CreateStreetcodeCoordinateRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    Latitude: MINLATITUDE,
                    Longtitude: longtitude);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Longtitude);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINSTREETCODEID - 10000)]
    public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new CreateStreetcodeCoordinateRequestDto(
                    StreetcodeId: id,
                    Longtitude: MINLONGTITUDE,
                    Latitude: MINLATITUDE);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateStreetcodeCoordinateRequestDto(
            StreetcodeId: MINSTREETCODEID,
            Longtitude: MINLONGTITUDE,
            Latitude: MINLATITUDE);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Latitude);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Longtitude);
    }
}
