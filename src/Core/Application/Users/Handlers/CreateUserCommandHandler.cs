using Application.Common.Results;
using Application.Common.Validators;
using Application.Users.Commands;
using Application.Users.Dtos;
using Domain.Authorization;
using Domain.Authorization.Repositories;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Handlers;

public class CreateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IAuthorizationRepository authorizationRepository,
    IPasswordValidator passwordValidator)
    : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Parola validasyonu
        var passwordValidation = passwordValidator.Validate(request.Password);
        if (!passwordValidation.IsSuccess)
        {
            return Result.Failure<UserDto>(Error.Validation(passwordValidation.Errors));
        }

        // Kullanıcı oluştur
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result.Failure<UserDto>(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        // Rolleri ata
        if (request.Roles.Any())
        {
            var roleResult = await userManager.AddToRolesAsync(user, request.Roles);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return Result.Failure<UserDto>(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }

        // Permission'ları ata
        foreach (var permission in request.Permissions)
        {
            var grant = PermissionGrant.Create(user.Id.ToString(), Guid.NewGuid(), permission);
            await authorizationRepository.AddGrantAsync(grant, cancellationToken);
        }

        // DTO dönüşümü ve response
        return Result.Success(new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            Roles = request.Roles,
            Permissions = request.Permissions
        });
    }
}