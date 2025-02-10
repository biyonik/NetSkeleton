using Application.Auth.Commands;
using Application.Common.Results;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Handlers;

public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager): IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userIsExistByEmail = await userManager.FindByEmailAsync(request.Email);
        
        if (userIsExistByEmail != null)
            return Result.Failure("Kullanıcı zaten mevcut");
        
        var userIsExistByUserName = await userManager.FindByNameAsync($"{request.FirstName.ToLowerInvariant()}.{request.LastName.ToLowerInvariant()}");
        if (userIsExistByUserName != null)
            return Result.Failure("Kullanıcı adı zaten mevcut");

        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = $"{request.FirstName.ToLowerInvariant()}.{request.LastName.ToLowerInvariant()}",
        };
        
        var createResult = await userManager.CreateAsync(user, request.Password);
        
        // TODO: Eğer kullanıcı oluşturulur ise, kullanıcıya mail atılacak
        
        return !createResult.Succeeded 
            ? Result.Failure(string.Join(", ", createResult.Errors.Select(e => e.Description))) 
            : Result.Success("Kullanıcı başarıyla oluşturuldu");
    }
}