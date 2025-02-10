using Application.Auth.Commands;
using Application.Common.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Auth.Validators;

public class RegisterCommandValidator: AbstractValidator<RegisterCommand>
{
    private readonly IPasswordValidator _passwordValidator;
    public RegisterCommandValidator(IPasswordValidator passwordValidator)
    {
        _passwordValidator = passwordValidator;
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email boş olamaz!")
            .EmailAddress()
            .WithMessage("Geçerli bir email adresi giriniz!");
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("İsim boş olamaz!")
            .MaximumLength(32)
            .WithMessage("İsim en fazla 32 karakter olabilir!");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Soyisim boş olamaz!")
            .MaximumLength(32)
            .WithMessage("Soyisim en fazla 32 karakter olabilir!");
        
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