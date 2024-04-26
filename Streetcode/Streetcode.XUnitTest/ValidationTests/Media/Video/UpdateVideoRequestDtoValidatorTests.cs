using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Media.Video.Update;

namespace Streetcode.XUnitTest.ValidationTests.Media.Video.Create
{
    public class UpdateVideoRequestDtoValidatorTests
    {
        private const int MAXTITLELENGTH = 100;
        private const int MAXDESCRPTIONLENGTH = 500;

        private readonly UpdateVideoRequestDtoValidator _validator;

        public UpdateVideoRequestDtoValidatorTests()
        {
            _validator = new UpdateVideoRequestDtoValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ShouldHaveErrors_WhenIdIsNull(int id)
        {
            // Arrange
            var dto = new UpdateVideoRequestDto(
                Id: id,
                Title: "Title",
                Description: "Description",
                Url: "https://www.youtube.com");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData(MAXTITLELENGTH + 1)]
        [InlineData(MAXTITLELENGTH + 10000)]
        public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
        {
            // Arrange
            var title = new string('a', length);
            var dto = new UpdateVideoRequestDto(
                Id: 1,
                Title: title,
                Description: "Description",
                Url: "https://www.youtube.com");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData(MAXDESCRPTIONLENGTH + 1)]
        [InlineData(MAXDESCRPTIONLENGTH + 10000)]
        public void ShouldHaveError_WhenDescriptionIsLongerThanAllowed(int length)
        {
            // Arrange
            var description = new string('a', length);
            var dto = new UpdateVideoRequestDto(
                Id: 1,
                Title: "Title",
                Description: description,
                Url: "https://www.youtube.com");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData("https://www.youtube.com")]
        [InlineData("http://www.youtube.com")]
        [InlineData("https://youtube.com")]
        [InlineData("http://youtube.com")]
        public void ShouldNotHaveError_WhenUrlIsValid(string url)
        {
            // Arrange
            var dto = new UpdateVideoRequestDto(
                Id: 1,
                Title: "Title",
                Description: "Description",
                Url: url);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.Url);
        }

        [Theory]
        [InlineData("https://www.example.com")]
        [InlineData("http://example.com")]
        [InlineData("https://example.com")]
        [InlineData("http://www.example.com")]
        public void ShouldHaveError_WhenUrlIsNotYoutube(string url)
        {
            // Arrange
            var dto = new UpdateVideoRequestDto(
                Id: 1,
                Title: "Title",
                Description: "Description",
                Url: url);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Url)
                .WithErrorMessage("Only youtube.com links are accepted.");
        }
    }
}
