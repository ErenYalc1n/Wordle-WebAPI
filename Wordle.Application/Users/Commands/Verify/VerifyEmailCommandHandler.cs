using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new Exception("Geçersiz kullanıcı.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new Exception("Kullanıcı bulunamadı.");

        if (user.EmailVerificationCode != request.Code || user.EmailVerificationExpiresAt < DateTime.UtcNow)
            throw new Exception("Geçersiz ya da süresi dolmuş doğrulama kodu.");

        user.Role = Role.Player;
        user.IsEmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpiresAt = null;

        var tokens = _tokenService.CreateToken(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _userRepository.UpdateAsync(user);

        return new AuthResultDto
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }
}
