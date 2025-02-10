using Application.Auth.Commands;
using Application.Common.Results;
using Application.Common.Security.Response;
using Application.Common.Security.Services;
using Domain.Identity.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Handlers;

public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService)
    : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Kullanıcıyı bul
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Failure<TokenResponse>("Böyle bir kullanıcı bulunamadı");

        // Kullanıcı aktif mi?
        if (!user.IsActive)
            return Result.Failure<TokenResponse>("Account is deactivated");

        // Parola doğrulaması
        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            // Başarısız giriş denemesi
            await userManager.AccessFailedAsync(user);
            return Result.Failure<TokenResponse>("Kullanıcı bilgileri doğrulanamadı! Lütfen tekrar deneyiniz.");
        }

        // Token üret
        var token = await jwtService.GenerateTokenAsync(user);
        
        // Başarılı giriş
        await userManager.ResetAccessFailedCountAsync(user);
        return Result.Success(token);
    }
}