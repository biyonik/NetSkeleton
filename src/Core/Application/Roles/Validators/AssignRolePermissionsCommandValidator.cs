using Application.Roles.Commands;
using FluentValidation;

namespace Application.Roles.Validators;

public class AssignRolePermissionsCommandValidator : AbstractValidator<AssignRolePermissionsCommand>
{
    public AssignRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions list cannot be null");
    }
}