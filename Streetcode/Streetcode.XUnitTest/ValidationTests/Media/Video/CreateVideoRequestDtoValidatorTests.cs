using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.DTO.Media.Video;

namespace Streetcode.XUnitTest.ValidationTests.Media.Video.Create
{
    public class CreateVideoRequestDtoValidatorTests
    {
        private const int MAXTITLELENGTH = 100;
        private const int MAXDESCRPTIONLENGTH = 500;
        private const int MINSTREETCODEID = 1;

        private readonly CreateVideoRequestDtoValidator _validator;

        public CreateVideoRequestDtoValidatorTests()
        {
            _validator = new CreateVideoRequestDtoValidator();
        }

        [Theory]
        [InlineData(MAXTITLELENGTH + 1)]
        [InlineData(MAXTITLELENGTH + 10000)]
        public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
        {
            // Arrange
            var title = new string('a', length);
            var dto = new CreateVideoRequestDto(
                Title: title,
                Description: "Description",
                Url: "https://www.youtube.com",
                StreetcodeId: MINSTREETCODEID);

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
            var dto = new CreateVideoRequestDto(
                Title: "Title",
                Description: description,
                Url: "https://www.youtube.com",
                StreetcodeId: MINSTREETCODEID);

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
            var dto = new CreateVideoRequestDto(
                Title: "Title",
                Description: "Description",
                Url: url,
                StreetcodeId: MINSTREETCODEID);

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
            var dto = new CreateVideoRequestDto(
                Title: "Title",
                Description: "Description",
                Url: url,
                StreetcodeId: MINSTREETCODEID);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Url)
                .WithErrorMessage("Only youtube.com links are accepted.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
        {
            // Arrange
            var dto = new CreateVideoRequestDto(
                Title: "Title",
                Description: "Description",
                Url: "https://www.youtube.com",
                StreetcodeId: id);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }
    }
}
