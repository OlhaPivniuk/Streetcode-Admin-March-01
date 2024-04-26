using FluentValidation;
using Streetcode.BLL.Dto.Account;

namespace Streetcode.BLL.MediatR.Account.Register;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public const int MAXNAMELENGTH = 50;
    public const int MINPASSWORDLENGTH = 6;
    public RegisterUserDtoValidator()
    {
        RuleFor(dto => dto.FirstName)
            .NotEmpty()
            .MaximumLength(MAXNAMELENGTH);

        RuleFor(dto => dto.LastName)
            .NotEmpty()
            .MaximumLength(MAXNAMELENGTH);

        RuleFor(dto => dto.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(dto => dto.Password)
            .MinimumLength(MINPASSWORDLENGTH);

        RuleFor(dto => dto.Password)
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,}$")
            .WithMessage($"Password must contain at least {MINPASSWORDLENGTH} characters, at least one uppercase letter, one lowercase letter, one number and one special character");
    }
}
