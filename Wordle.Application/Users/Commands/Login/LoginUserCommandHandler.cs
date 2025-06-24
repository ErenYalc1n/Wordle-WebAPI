using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdentifierAsync(request.Identifier);

        if (user is null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        if (user.PasswordHash != FakeHash(request.Password))
            throw new UnauthorizedAccessException("Şifre hatalı.");

        var tokens = _tokenService.CreateToken(user);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _userRepository.UpdateAsync(user);

        return new LoginResultDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }

    private string FakeHash(string password)
    {
        return $"HASHED::{password}";
    }
}
