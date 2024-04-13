﻿using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.Streetcode.Text;

public class CreateTextRequestDtoValidatorTests
{
    private const int MINSTREETCODEID = 1;
    private const int MINTITLELENGTH = 1;
    private const int MINTEXTCONTENTLENGTH = 1;
    private const int MAXTITLELENGTH = 50;
    private const int MAXTEXTCONTENTLENGTH = 15000;
    private const int MAXADDITIONALTEXTLENGTH = 200;

    private readonly CreateTextRequestDtoValidator _validator;

    public CreateTextRequestDtoValidatorTests()
    {
        _validator = new CreateTextRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINSTREETCODEID - 10000)]
    public void Should_have_error_when_StreetcodeId_is_zero_or_negative(int id)
    {
        // Arrange
        var dto = new CreateTextRequestDto(
                    StreetcodeId: id,
                    Title: new string('a', MINTITLELENGTH),
                    TextContent: new string('a', MINTEXTCONTENTLENGTH),
                    AdditionalText: new string('a', 1));

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MAXTITLELENGTH + 10000)]
    public void Should_have_error_when_Title_length_is_greater_than_MAXTITLE_or_equel_Zero(int number)
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
    public void Should_have_error_when_TextContent_length_is_greater_than_MAXTEXTCONTENTLENGTH_or_equel_Zero(int number)
    {
        // Arrange
        var dto = new CreateTextRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    Title: new string('a', MINTITLELENGTH ),
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
    public void Should_have_error_when_AdditionalText_length_is_greater_than_MAXADDITIONALTEXTLENGTH(int number)
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
    public void Should_not_have_error_when_dto_is_valid()
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
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Title);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.TextContent);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.AdditionalText);
    }
}