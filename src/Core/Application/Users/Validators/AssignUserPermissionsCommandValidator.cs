using Application.Users.Commands;
using FluentValidation;

namespace Application.Users.Validators;

public class AssignUserPermissionsCommandValidator : AbstractValidator<AssignUserPermissionsCommand>
{
    public AssignUserPermissionsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions list cannot be null");
    }
}