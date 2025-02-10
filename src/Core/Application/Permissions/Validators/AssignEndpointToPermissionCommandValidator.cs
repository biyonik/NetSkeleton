using Application.Permissions.Commands;
using FluentValidation;

namespace Application.Permissions.Validators;

public class AssignEndpointToPermissionCommandValidator 
    : AbstractValidator<AssignEndpointToPermissionCommand>
{
    public AssignEndpointToPermissionCommandValidator()
    {
        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("Permission ID is required");

        RuleFor(x => x.Controller)
            .NotEmpty().WithMessage("Controller name is required")
            .MaximumLength(100).WithMessage("Controller name cannot exceed 100 characters");

        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action name is required")
            .MaximumLength(100).WithMessage("Action name cannot exceed 100 characters");

        RuleFor(x => x.HttpMethod)
            .NotEmpty().WithMessage("HTTP method is required")
            .MaximumLength(10).WithMessage("HTTP method cannot exceed 10 characters");

        RuleFor(x => x.Route)
            .NotEmpty().WithMessage("Route is required")
            .MaximumLength(500).WithMessage("Route cannot exceed 500 characters");
    }
}