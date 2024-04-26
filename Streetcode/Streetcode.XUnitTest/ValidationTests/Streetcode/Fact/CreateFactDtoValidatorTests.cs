using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Fact
{
    public class CreateFactDtoValidatorTests
    {
        private const int MAXTITLELENGTH = 68;
        private const int MAXFACTCONTENTLENGTH = 600;
        private const int MINIMAGEID = 1;
        private const int MINSTREETCODEID = 1;

        private readonly CreateFactDtoValidator _validator;

        public CreateFactDtoValidatorTests()
        {
            _validator = new CreateFactDtoValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldHaveError_WhenTitleIsNullOrEmpty(string title)
        {
            // Arrange
            var dto = new CreateFactDto(
                Title: title,
                FactContent: "FactContent",
                ImageId: MINIMAGEID,
                StreetcodeId: MINSTREETCODEID);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData(MAXTITLELENGTH + 1)]
        [InlineData(MAXTITLELENGTH + 10000)]
        public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
        {
            // Arrange
            var title = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateFactDto(
                Title: title,
                ImageId: MINIMAGEID,
                StreetcodeId: MINSTREETCODEID,
                FactContent: "FactContent");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldHaveError_WhenFactContentIsNullOrEmpty(string factContent)
        {
            var dto = new CreateFactDto(
                Title: "Title",
                ImageId: MINIMAGEID,
                StreetcodeId: MINSTREETCODEID,
                FactContent: factContent);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.FactContent);
        }

        [Theory]
        [InlineData(MAXFACTCONTENTLENGTH + 1)]
        [InlineData(MAXFACTCONTENTLENGTH + 10000)]
        public void ShouldHaveError_WhenFactContentIsLongerThanAllowed(int length)
        {
            // Arrange
            var text = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateFactDto(
                    Title: "Title",
                    ImageId: MINIMAGEID,
                    StreetcodeId: MINSTREETCODEID,
                    FactContent: text);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.FactContent);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MINIMAGEID - 10000)]
        public void ShouldHaveError_WhenImageIdIsZeroOrNegative(int id)
        {
            // Arrange
            var dto = new CreateFactDto(
                        Title: "Title",
                        ImageId: id,
                        StreetcodeId: MINSTREETCODEID,
                        FactContent: "FactContent");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.ImageId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MINSTREETCODEID - 10000)]
        public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
        {
            // Arrange
            var dto = new CreateFactDto(
                        Title: "Title",
                        ImageId: MINIMAGEID,
                        StreetcodeId: id,
                        FactContent: "FactContent");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void ShouldNotHaveError_WhenDtoIsValid()
        {
            // Arrange
            var dto = new CreateFactDto(
                Title: "Title",
                ImageId: MINIMAGEID,
                StreetcodeId: MINSTREETCODEID,
                FactContent: "FactContent");

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
            validationResult.ShouldNotHaveValidationErrorFor(x => x.ImageId);
            validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
            validationResult.ShouldNotHaveValidationErrorFor(x => x.FactContent);
        }
    }
}
