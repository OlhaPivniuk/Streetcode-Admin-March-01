﻿using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.StreetcodeToponym;
using Streetcode.BLL.MediatR.StreetcodeToponym.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidationTests.StreetcodeToponym;

public class DeleteStreetcodeToponymRequestDtoValidatorTests
{
    private const int MINSTREETCODEID = 1;
    private const int MINTOPONYMID = 1;

    private readonly DeleteStreetcodeToponymRequestDtoValidator _validator;

    public DeleteStreetcodeToponymRequestDtoValidatorTests()
    {
        _validator = new DeleteStreetcodeToponymRequestDtoValidator();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINTOPONYMID - 10000)]
    public void Should_have_error_when_ToponymId_is_zero_or_negative(int id)
    {
        // Arrange
        var dto = new DeleteStreetcodeToponymRequestDto(
                    StreetcodeId: MINSTREETCODEID,
                    ToponymId: id);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ToponymId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(MINSTREETCODEID - 10000)]
    public void Should_have_error_when_StreetcodeId_is_zero_or_negative(int id)
    {
        // Arrange
        var dto = new DeleteStreetcodeToponymRequestDto(
                    StreetcodeId: id,
                    ToponymId: MINSTREETCODEID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
    }

    [Fact]
    public void Should_not_have_error_when_dto_is_valid()
    {
        // Arrange
        var dto = new DeleteStreetcodeToponymRequestDto(
            ToponymId: MINTOPONYMID,
            StreetcodeId: MINSTREETCODEID);

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ToponymId);
    }
}