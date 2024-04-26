using FluentValidation;
using Streetcode.BLL.DTO.Account;

namespace Streetcode.BLL.MediatR.Account.GenerateNewAccessToken;

public class TokenModelDtoValidator : AbstractValidator<TokenModelDto>
{
    public TokenModelDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
