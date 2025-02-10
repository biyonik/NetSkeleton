using Application.Permissions.Commands;
using FluentValidation;

namespace Application.Permissions.Validators;

public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required")
            .MaximumLength(100).WithMessage("Permission name cannot exceed 100 characters");

        RuleFor(x => x.SystemName)
            .NotEmpty().WithMessage("System name is required")
            .MaximumLength(100).WithMessage("System name cannot exceed 100 characters")
            .Matches("^[a-zA-Z0-9.]+$").WithMessage("System name can only contain letters, numbers and dots");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters");
    }
}
