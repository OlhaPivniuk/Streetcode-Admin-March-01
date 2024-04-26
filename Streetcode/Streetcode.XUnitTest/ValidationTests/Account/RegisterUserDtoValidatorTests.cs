using Xunit;
using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Account.Register;
using Streetcode.BLL.Dto.Account;

namespace Streetcode.XUnitTest.ValidationTests.Account;

public class RegisterUserDtoValidatorTests
{
    private const int MAXNAMELENGTH = 50;
    private const int MINPASSWORDLENGTH = 6;

    private readonly RegisterUserDtoValidator _validator;

    private readonly RegisterUserDto _validDto = new ()
    {
        FirstName = "John",
        LastName = "Smith",
        Email = "test@mail.com",
        Password = "String1@"
    };

    public RegisterUserDtoValidatorTests()
    {
        _validator = new RegisterUserDtoValidator();
    }

    [Theory]
    [InlineData(MAXNAMELENGTH + 1)]
    [InlineData(MAXNAMELENGTH + 10000)]
    public void ShouldHaveError_WhenFirstNameIsLongerThanAllowed(int length)
    {
        // Arrange
        var name = new string('a', length);
        var dto = new RegisterUserDto
        {
            FirstName = name,
            LastName = "Smith",
            Email = "test@mail.com",
            Password = "String1@"
        };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData(MAXNAMELENGTH + 1)]
    [InlineData(MAXNAMELENGTH + 10000)]
    public void ShouldHaveError_WhenLastNameIsLongerThanAllowed(int length)
    {
        // Arrange
        var name = new string('a', length);
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = name,
            Email = "test@mail.com",
            Password = "String1@"
        };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData(MINPASSWORDLENGTH - 1)]
    public void ShouldHaveError_WhenPasswordIsShorterThanAllowed(int length)
    {
        // Arrange
        var password = new string('a', length);
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "test@mail.com",
            Password = "Strin"
        };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("string")]
    public void ShouldHaveError_WhenPasswordDoesNotContainCapitalLetter(string password)
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "test@mail.com",
            Password = password
        };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("string")]
    public void ShouldHaveError_WhenPasswordDoesNotContainDigit(string password)
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "test@mail.com",
            Password = password
        };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("string")]
    public void ShouldHaveError_WhenPasswordDoesNotContainSpecialCharacter(string password)
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "test@mail.com",
            Password = password
        };

        // Act
        var validationResult = _validator.TestValidate(dto);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ShouldNotHaveErrorOnFirstName_WhenDtoIsValid()
    {
        // Arrange

        // Act
        var validationResult = _validator.TestValidate(_validDto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void ShouldNotHaveErrorOnLastName_WhenDtoIsValid()
    {
        // Arrange

        // Act
        var validationResult = _validator.TestValidate(_validDto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void ShouldNotHaveErrorOnEmail_WhenDtoIsValid()
    {
        // Arrange

        // Act
        var validationResult = _validator.TestValidate(_validDto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ShouldNotHaveErrorOnPassword_WhenDtoIsValid()
    {
        // Arrange

        // Act
        var validationResult = _validator.TestValidate(_validDto);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
