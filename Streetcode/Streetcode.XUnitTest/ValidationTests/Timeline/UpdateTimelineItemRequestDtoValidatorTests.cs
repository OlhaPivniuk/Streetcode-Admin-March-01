using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.BLL.Dto.Timeline;
using Streetcode.DAL.Enums;

namespace Streetcode.XUnitTest.ValidationTests.Timeline;

public class UpdateTimelineItemRequestDtoValidatorTests
{
    private const int MAXTITLELENGTH = 26;
    private const int MAXDESCRIPTIONLENGTH = 400;
    private const int MAXCONTEXTLENGTH = 50;

    private readonly UpdateTimelineItemRequestDtoValidator _validator;

    public UpdateTimelineItemRequestDtoValidatorTests()
    {
        _validator = new UpdateTimelineItemRequestDtoValidator();
    }

    [Theory]
    [InlineData(MAXTITLELENGTH + 1)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void ShouldHaveError_WhenTitleIsLongerThanAllowed(int length)
    {
        // Arrange
        var title = new string('a', length);
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: title,
            Description: "description",
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(MAXDESCRIPTIONLENGTH + 1)]
    [InlineData(MAXDESCRIPTIONLENGTH + 10000)]
    public void ShouldHaveError_WhenDescriptionIsLongerThanAllowed(int length)
    {
        // Arrange
        var description = new string('a', length);
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: description,
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(MAXCONTEXTLENGTH + 1)]
    [InlineData(MAXCONTEXTLENGTH + 10000)]
    public void ShouldHaveError_WhenHistoricalContextIsLongerThanAllowed(int length)
    {
        // Arrange
        var historicalContext = new string('a', length);
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: "description",
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: historicalContext);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.HistoricalContext);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldHaveErrors_WhenTitleIsNullOrEmpty(string title)
    {
        // Arrange
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: title,
            Description: "description",
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldHaveErrors_WhenDescriptionIsNullOrEmpty(string description)
    {
        // Arrange
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: description,
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(MAXTITLELENGTH)]
    [InlineData(MAXTITLELENGTH - 1)]
    [InlineData(MAXTITLELENGTH - 25)]
    public void ShouldNotHaveErrors_WhenTitleHasAllowedLength(int length)
    {
        // Arrange
        var title = new string('a', length);
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: title,
            Description: "description",
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(MAXDESCRIPTIONLENGTH)]
    [InlineData(MAXDESCRIPTIONLENGTH - 1)]
    [InlineData(MAXDESCRIPTIONLENGTH - 399)]
    public void ShouldNotHaveErrors_WhenDescriptionHasAllowedLength(int length)
    {
        // Arrange
        var description = new string('a', length);
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: description,
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: "");

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(MAXCONTEXTLENGTH)]
    [InlineData(MAXCONTEXTLENGTH - 1)]
    [InlineData(MAXCONTEXTLENGTH - 49)]
    public void ShouldNotHaveErrors_WhenContextHasAllowedLength(int length)
    {
        // Arrange
        var context = new string('a', length);
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: "description",
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: context);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.HistoricalContext);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ShouldNotHaveErrors_WhenContextIsNullOrEmpty(string context)
    {
        // Arrange
        var dto = new UpdateTimelineItemRequestDto(
            Id: 1,
            StreetcodeId: 1,
            Title: "title",
            Description: "description",
            Date: DateTime.UtcNow,
            DateViewPattern: DateViewPattern.DateMonthYear,
            HistoricalContext: context);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.HistoricalContext);
    }
}
