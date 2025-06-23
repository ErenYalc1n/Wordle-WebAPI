using MediatR;

namespace Wordle.Application.Users
{
    public class LogoutUserCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
