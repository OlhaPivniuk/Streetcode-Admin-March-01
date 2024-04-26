using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners.Delete;
using Streetcode.BLL.MediatR.Partners.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Partners;

public class DeletePartnerRequestDtoValidatorTests
{
    private const int MINID = 1;

    private readonly DeletePartnerRequestDtoValidator _validator;

    public DeletePartnerRequestDtoValidatorTests()
    {
        _validator = new DeletePartnerRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINID - 10000)]
    public void ShouldHaveError_WhenIdIsZeroOrNegative(int id)
    {
        // Arrange
        var dto = new DeletePartnerRequestDto(Id: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new DeletePartnerRequestDto(Id: MINID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}