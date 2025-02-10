using Application.Auth.Commands;
using FluentValidation;

namespace Application.Auth.Validators;

public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID gereklidir!");
    }
}