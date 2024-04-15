﻿using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Text;

public class PreviewTextRequestDtoValidatorTests
{
    private const int MINSTREETCODEID = 1;
    private const int MINTITLELENGTH = 1;
    private const int MINTEXTCONTENTLENGTH = 1;
    private const int MAXTITLELENGTH = 50;
    private const int MAXTEXTCONTENTLENGTH = 15000;
    private const int MAXADDITIONALTEXTLENGTH = 200;

    private readonly CreateTextRequestDtoValidator _validator;

    public PreviewTextRequestDtoValidatorTests()
    {
        _validator = new CreateTextRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void ShouldHaveError_WhenTitleIsGreaterThanAllowedOrZero(int number)
    {
        // Arrange
        var dto = new CreateTextRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    Title: new string('a', number),
                    TextContent: new string('a', MINTEXTCONTENTLENGTH),
                    AdditionalText: new string('a', 1));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXTEXTCONTENTLENGTH + 10000)]
    public void ShouldHaveError_WhenTextContentIsGreaterThanAllowedOrZero(int number)
    {
        // Arrange
        var dto = new CreateTextRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    Title: new string('a', MINTITLELENGTH),
                    TextContent: new string('a', number),
                    AdditionalText: new string('a', 1));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.TextContent);
    }

    [Theory]
    [InlineData(MAXADDITIONALTEXTLENGTH + 10)]
    [InlineData(MAXADDITIONALTEXTLENGTH + 10000)]
    public void ShouldHaveError_WhenAdditionalTextIsGreaterThanAllowed(int number)
    {
        // Arrange
        var dto = new CreateTextRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    Title: new string('a', MINTITLELENGTH),
                    TextContent: new string('a', MINTEXTCONTENTLENGTH),
                    AdditionalText: new string('a', number));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.AdditionalText);
    }

    [Fact]
    public void ShouldNotHaveError_WhenDtoIsValid()
    {
        // Arrange
        var dto = new CreateTextRequestDto(
            StreetcodeId: MINSTREETCODEID,
            Title: new string('a', MINTITLELENGTH),
            TextContent: new string('a', MINTEXTCONTENTLENGTH),
            AdditionalText: new string('a', 1));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.TextContent);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.AdditionalText);
    }
}