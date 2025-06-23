using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands;

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

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(21);

        await _userRepository.UpdateAsync(user);

        return new LoginResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

    }
}
