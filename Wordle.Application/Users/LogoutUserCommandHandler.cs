using MediatR;
using Wordle.Domain.Users;

namespace Wordle.Application.Users
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public LogoutUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<Unit> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user is null)
                    throw new Exception("Kullanıcı bulunamadı.");

                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;

                await _userRepository.UpdateAsync(user);

                return Unit.Value;
            }, cancellationToken);
        }
    }
}
