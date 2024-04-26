using FluentValidation;
using Streetcode.BLL.DTO.Account;

namespace Streetcode.BLL.MediatR.Account.Logout;

public class LogoutUserDtoValidator : AbstractValidator<LogoutUserDto>
{
    public LogoutUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
