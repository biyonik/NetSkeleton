using Application.Auth.Commands;
using Application.Common.Results;
using Application.Common.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Auth.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private readonly IPasswordValidator _passwordValidator;

    public LoginCommandValidator(IPasswordValidator passwordValidator)
    {
        _passwordValidator = passwordValidator;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta alanı boş bırakılamaz")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.Password)
            .Custom((password, context) =>
            {
                var result = _passwordValidator.Validate(password);
                if (result.IsSuccess) return;
                foreach (var message in result.Errors.SelectMany(entry => entry.Value))
                {
                    context.AddFailure(new ValidationFailure("Password", message));
                }
            });
    }
}