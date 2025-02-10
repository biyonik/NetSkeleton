using Application.Users.Commands;
using FluentValidation;

namespace Application.Users.Validators;

public class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Roles)
            .NotNull().WithMessage("Roles list cannot be null");
    }
}