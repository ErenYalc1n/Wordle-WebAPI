using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);

        if (user is null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new Exception("Geçersiz ya da süresi dolmuş refresh token.");

        var tokens = _tokenService.CreateToken(user);

        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _userRepository.UpdateAsync(user);

        return new LoginResultDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
    }
}
