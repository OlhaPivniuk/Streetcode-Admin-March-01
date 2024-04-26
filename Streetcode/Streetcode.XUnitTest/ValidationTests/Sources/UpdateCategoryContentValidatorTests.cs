using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.XUnitTest.ValidationTests.Sources
{
    public class UpdateCategoryContentValidatorTests
    {
        private const int MAXTEXTLENGTH = 4000;
        private const int MINSOURCELINKCATEGORYID = 1;
        private const int MINSTREETCODEID = 1;

        private readonly UpdateCategoryContentValidator _validator;

        public UpdateCategoryContentValidatorTests()
        {
            _validator = new UpdateCategoryContentValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldHaveError_WhenTextIsNullOrEmpty(string text)
        {
            // Arrange
            var dto = new CategoryContentUpdateDto(
                Text: text,
                SourceLinkCategoryId: MINSOURCELINKCATEGORYID,
                StreetcodeId: MINSTREETCODEID);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Text);
        }

        [Theory]
        [InlineData(MAXTEXTLENGTH + 1)]
        [InlineData(MAXTEXTLENGTH + 10000)]
        public void ShouldHaveError_WhenTextIsLongerThanAllowed(int length)
        {
            // Arrange
            var text = new string('a', length);
            var dto = new CategoryContentUpdateDto(
                Text: text,
                SourceLinkCategoryId: MINSOURCELINKCATEGORYID,
                StreetcodeId: MINSTREETCODEID);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Text);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MINSOURCELINKCATEGORYID - 10000)]
        public void ShouldHaveError_WhenSourceLinkCategoryIdIsZeroOrNegative(int id)
        {
            // Arrange
            var dto = new CategoryContentUpdateDto(
                Text: "Text",
                SourceLinkCategoryId: id,
                StreetcodeId: MINSTREETCODEID);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.SourceLinkCategoryId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(MINSTREETCODEID - 10000)]
        public void ShouldHaveError_WhenStreetcodeIdIsZeroOrNegative(int id)
        {
            // Arrange
            var dto = new CategoryContentUpdateDto(
                Text: "Text",
                SourceLinkCategoryId: MINSOURCELINKCATEGORYID,
                StreetcodeId: id);

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }
    }
}
