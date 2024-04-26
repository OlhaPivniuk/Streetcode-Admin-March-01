using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Analytics.StatisticRecord;
using Streetcode.BLL.MediatR.Analytics.StatisticRecord.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Analytics.StatisticRecord;

public class DeleteStatisticRecordRequestDtoValidatorTests
{
    private const int MINID = 1;

    private readonly DeleteStatisticRecordRequestDtoValidator _validator;

    public DeleteStatisticRecordRequestDtoValidatorTests()
    {
        _validator = new DeleteStatisticRecordRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINID - 10000)]
    public void ShouldHaveError_WhenIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new DeleteStatisticRecordRequestDto(Id: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new DeleteStatisticRecordRequestDto(Id: MINID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}