using MediatR;

namespace Wordle.Application.Users.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResultDto>;
}
