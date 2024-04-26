using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Media.Video.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Media.Video
{
    public class DeleteVideoRequestDtoValidatorTests
    {
        private const int MINID = 1;

        private readonly DeleteVideoRequestDtoValidator _validator;

        public DeleteVideoRequestDtoValidatorTests()
        {
            _validator = new DeleteVideoRequestDtoValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MINID - 10000)]
        public void ShouldHaveError_WhenIdIsZeroOrNegative(int id)
        {
            // Arrange
            var dto = new DeleteVideoRequestDto(Id: id);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void ShouldNotHaveError_WhenDtoIsValid()
        {
            // Arrange
            var dto = new DeleteVideoRequestDto(Id: MINID);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}
