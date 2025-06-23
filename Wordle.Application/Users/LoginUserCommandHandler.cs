using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.Commands;
using Wordle.Domain.Users;

namespace Wordle.Application.Users
{
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

            if (!user.IsEmailConfirmed)
                throw new UnauthorizedAccessException("E-posta adresi onaylanmamış.");

            if (user.PasswordHash != FakeHash(request.Password))
                throw new UnauthorizedAccessException("Şifre hatalı.");


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

        private string FakeHash(string password)
        {
            return $"HASHED::{password}";
        }
    }
}
