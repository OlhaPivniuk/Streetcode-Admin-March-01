using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Analytics.StatisticRecord;

public class CreateStatisticRecordRequestDtoValidatorTests
{
    private const int MINSTREETCODEID = 1;
    private const int MINSTREETCODECOORDINATEID = 1;
    private const int MAXADDRESSLENGTH = 150;
    private const int MINADDRESSLENGTH = 1;

    private readonly CreateStatisticRecordRequestDtoValidator _validator;

    public CreateStatisticRecordRequestDtoValidatorTests()
    {
        _validator = new CreateStatisticRecordRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINSTREETCODEID - 10000)]
    public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new CreateStatisticRecordRequestDto(
                    StreetcodeId: id,
                    StreetcodeCoordinateId: MINSTREETCODECOORDINATEID,
                    Address: new string('a', MINADDRESSLENGTH));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINSTREETCODECOORDINATEID - 10000)]
    public void ShouldHaveError_WhenStreetcodeCoordinateIdIsZeroOrNegative(int streetcodeCoordinateId)
    {
        // Arrange
        var dto = new CreateStatisticRecordRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    StreetcodeCoordinateId: streetcodeCoordinateId,
                    Address: new string('a', MINADDRESSLENGTH));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeCoordinateId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXADDRESSLENGTH + 10000)]
    public void ShouldHaveError_WhenAddressIsGreaterThanAllowedOrZero(int id)
    {
        // Arrange
        var dto = new CreateStatisticRecordRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    StreetcodeCoordinateId: MINSTREETCODECOORDINATEID,
                    Address: new string('a', id));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateStatisticRecordRequestDto(
            StreetcodeId: MINSTREETCODEID,
            StreetcodeCoordinateId: MINSTREETCODECOORDINATEID,
            Address: new string('a', MINADDRESSLENGTH));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeCoordinateId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Address);
    }
}