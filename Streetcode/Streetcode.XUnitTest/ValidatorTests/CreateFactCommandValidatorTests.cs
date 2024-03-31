using FluentValidation.TestHelper;
using Streetcode.BLL.Dto.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests;

public class CreateFactCommandValidatorTests
{
    private readonly CreateFactCommandValidator _validator;
    private readonly int _maxTitleLength;
    private readonly int _maxFactContentLength;

    public CreateFactCommandValidatorTests()
    {
        _validator = new CreateFactCommandValidator();
        _maxTitleLength = 68;
        _maxFactContentLength = 600;
    }

    [Fact]
    public void Should_have_error_when_Title_is_empty()
    {
        // Arrange
        var command = new CreateFactCommand(GetCreateFactDtoWithEmptyTitle());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Request.Title);
    }

    [Theory]
    [InlineData(69)]
    [InlineData(169)]
    public void Should_have_error_when_Title_is_longer_than_maxTitleLength_rule(int length)
    {
        // Arrange
        var command = new CreateFactCommand(GetCreateFactDtoWithVeryLongTitle(length));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Request.Title);
    }

    [Fact]
    public void Should_have_error_when_FactContent_is_empty()
    {
        // Arrange
        var command = new CreateFactCommand(GetCreateFactDtoWithEmptyFactContent());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Request.FactContent);
    }

    [Theory]
    [InlineData(800)]
    [InlineData(601)]
    public void Should_have_error_when_FactContent_is_longer_than_maxFactContent_rule(int length)
    {
        // Arrange
        var command = new CreateFactCommand(GetCreateFactDtoWithVeryLongFactContent(length));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Request.FactContent);
    }

    [Fact]
    public void Should_have_error_when_ImageId_is_Zero()
    {
        // Arrange
        var command = new CreateFactCommand(GetCreateFactDtoWithZeroImageId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Request.ImageId);
    }

    [Fact]
    public void Should_have_error_when_StreetcodeId_is_Zero()
    {
        // Arrange
        var command = new CreateFactCommand(GetCreateFactDtoWithZeroStreetcodeId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(command => command.Request.StreetcodeId);
    }

    [Fact]
    public void Should_not_have_errors_when_Dto_is_valid()
    {
        // Arrange
        var command = new CreateFactCommand(GetValidCreateFactDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_not_have_FactContent_error_when_FactContent_is_correct()
    {
        // Arrange
        var command = new CreateFactCommand(GetValidCreateFactDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(command => command.Request.FactContent);
    }

    [Fact]
    public void Should_not_have_ImageId_error_when_ImageId_is_correct()
    {
        // Arrange
        var command = new CreateFactCommand(GetValidCreateFactDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(command => command.Request.ImageId);
    }

    [Fact]
    public void Should_not_have_StreetcodeId_error_when_StreetcodeId_is_correct()
    {
        // Arrange
        var command = new CreateFactCommand(GetValidCreateFactDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(command => command.Request.StreetcodeId);
    }

    private static CreateFactDto GetCreateFactDtoWithEmptyTitle()
    {
        return new CreateFactDto("", "fact content", 1, 1);
    }

    private static CreateFactDto GetCreateFactDtoWithEmptyFactContent()
    {
        return new CreateFactDto("fact", "", 1, 1);
    }

    private static CreateFactDto GetCreateFactDtoWithVeryLongTitle(int length)
    {
        return new CreateFactDto(string.Concat(Enumerable.Repeat('a', length)), "fact content", 1, 1);
    }

    private static CreateFactDto GetCreateFactDtoWithVeryLongFactContent(int length)
    {
        return new CreateFactDto("title", string.Concat(Enumerable.Repeat('a', length)), 1, 1);
    }

    private static CreateFactDto GetCreateFactDtoWithZeroImageId()
    {
        return new CreateFactDto("fact", "fact content", 0, 1);
    }

    private static CreateFactDto GetCreateFactDtoWithZeroStreetcodeId()
    {
        return new CreateFactDto("fact", "fact content", 1, 0);
    }

    private static CreateFactDto GetValidCreateFactDto()
    {
        return new CreateFactDto("fact", "fact content", 1, 1);
    }
}
