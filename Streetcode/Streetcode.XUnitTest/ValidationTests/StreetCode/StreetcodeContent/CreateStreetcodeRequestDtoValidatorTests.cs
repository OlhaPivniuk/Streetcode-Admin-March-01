using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.Create;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Streetcode
{
    public class CreateStreetcodeRequestDtoValidatorTests
    {
        private const int MAXTITLELENGTH = 100;
        private const int MAXFIRSTNAMELENGTH = 50;
        private const int MAXLASTNAMELENGTH = 50;
        private const int MAXALIASLENGTH = 33;
        private const int MAXNUMBEROFTAGS = 50;
        private const int MAXTEASERLENGTH = 450;
        private const int MAXTRANSLITERATIONURLLENGTH = 100;

        private readonly CreateStreetcodeRequestDtoValidator _validator;

        public CreateStreetcodeRequestDtoValidatorTests()
        {
            _validator = new CreateStreetcodeRequestDtoValidator();
        }

        [Fact]
        public void ShouldHaveError_WhenStreetcodeTypeIsInvalid()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = (StreetcodeType)100,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName"
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeType);
        }

        [Theory]
        [InlineData(MAXTITLELENGTH + 1)]
        [InlineData(MAXTITLELENGTH + 10000)]
        public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
        {
            // Arrange
            var title = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = title,
                FirstName = "FirstName",
                LastName = "LastName"
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void ShouldHaveError_WhenFirstNameIsMissing()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                LastName = "LastName"
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void ShouldHaveError_WhenLastNameIsMissing()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName"
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Theory]
        [InlineData(MAXFIRSTNAMELENGTH + 1)]
        [InlineData(MAXFIRSTNAMELENGTH + 10000)]
        public void ShouldHaveError_WhenFirstNameIsLongerThanAllowed(int length)
        {
            // Arrange
            var firstName = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = firstName,
                LastName = "LastName"
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Theory]
        [InlineData(MAXLASTNAMELENGTH + 1)]
        [InlineData(MAXLASTNAMELENGTH + 10000)]
        public void ShouldHaveError_WhenLastNameIsLongerThanAllowed(int length)
        {
            // Arrange
            var lastName = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = lastName
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void ShouldNotHaveError_WhenEventEndOrPersonDeathDateIsNull()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                EventStartOrPersonBirthDate = new DateTime(1990, 1, 1),
                EventEndOrPersonDeathDate = null // Nullable
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.EventEndOrPersonDeathDate);
        }

        [Fact]
        public void ShouldNotHaveError_WhenEventEndOrPersonDeathDateIsAfterEventStartOrPersonBirthDate()
        {
            // Arrange
            var eventStart = new DateTime(1990, 1, 1);
            var eventEnd = new DateTime(2000, 1, 1);
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                EventStartOrPersonBirthDate = eventStart,
                EventEndOrPersonDeathDate = eventEnd
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.EventEndOrPersonDeathDate);
        }

        [Fact]
        public void ShouldHaveError_WhenEventEndOrPersonDeathDateIsBeforeEventStartOrPersonBirthDate()
        {
            // Arrange
            var eventStart = new DateTime(2000, 1, 1);
            var eventEnd = new DateTime(1990, 1, 1);
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                EventStartOrPersonBirthDate = eventStart,
                EventEndOrPersonDeathDate = eventEnd
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.EventEndOrPersonDeathDate);
        }

        [Theory]
        [InlineData(MAXALIASLENGTH + 1)]
        [InlineData(MAXALIASLENGTH + 10000)]
        public void ShouldHaveError_WhenAliasIsLongerThanAllowed(int length)
        {
            // Arrange
            var alias = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                Alias = alias
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Alias);
        }

        [Fact]
        public void ShouldNotHaveError_WhenTagIdsCountIsLessThanOrEqualToMaxNumberOfTags()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                TagIds = Enumerable.Range(1, MAXNUMBEROFTAGS)
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.TagIds);
        }

        [Fact]
        public void ShouldHaveError_WhenNumberOfTagsExceedsMaxNumberOfTags()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                TagIds = Enumerable.Range(1, MAXNUMBEROFTAGS + 1)
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.TagIds);
        }

        [Theory]
        [InlineData(MAXTEASERLENGTH + 1)]
        [InlineData(MAXTEASERLENGTH + 10000)]
        public void ShouldHaveError_WhenTeaserExceedsMaxTeaserLength(int length)
        {
            // Arrange
            var teaser = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                Teaser = teaser
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.Teaser);
        }

        [Fact]
        public void ShouldNotHaveError_WhenImageIdsCountIsLessThanOrEqualToTwo()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                ImageIds = new List<int> { 1, 2 }
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.ImageIds);
        }

        [Fact]
        public void ShouldHaveError_WhenImageIdsCountIsMoreThanTwo()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                ImageIds = new List<int> { 1, 2, 3 }
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.ImageIds);
        }

        [Fact]
        public void ShouldHaveError_WhenImageIdsContainDuplicates()
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                ImageIds = new List<int> { 1, 2, 2 }
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.ImageIds);
        }

        [Theory]
        [InlineData(MAXTRANSLITERATIONURLLENGTH + 1)]
        [InlineData(MAXTRANSLITERATIONURLLENGTH + 10000)]
        public void ShouldHaveError_WhenTransliterationUrlExceedsMaxTransliterationUrlLength(int length)
        {
            // Arrange
            var transliterationUrl = string.Concat(Enumerable.Repeat('a', length));
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                TransliterationUrl = transliterationUrl
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.TransliterationUrl);
        }

        [Theory]
        [InlineData("valid-url")]
        [InlineData("valid-123-url")]
        public void ShouldNotHaveError_WhenTransliterationUrlMatchesPattern(string transliterationUrl)
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                TransliterationUrl = transliterationUrl
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldNotHaveValidationErrorFor(x => x.TransliterationUrl);
        }

        [Theory]
        [InlineData("in valid-url")]
        [InlineData("invalid@url")]
        [InlineData("invalid.url")]
        [InlineData("INVALID-URL")]
        public void ShouldHaveError_WhenTransliterationUrlDoesNotMatchPattern(string transliterationUrl)
        {
            // Arrange
            var dto = new CreateStreetcodeRequestDto
            {
                StreetcodeType = StreetcodeType.Person,
                Title = "Title",
                FirstName = "FirstName",
                LastName = "LastName",
                TransliterationUrl = transliterationUrl
            };

            // Act
            var validationResult = _validator.TestValidate(dto);

            // Assert
            validationResult.ShouldHaveValidationErrorFor(x => x.TransliterationUrl);
        }
    }
}
