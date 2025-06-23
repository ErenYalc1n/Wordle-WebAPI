using MediatR;

namespace Wordle.Application.Users
{
    public record RegisterUserCommand(
        string Email,
        string Password,
        string Nickname,
        string? FirstName,
        string? LastName,
        bool IsKvkkAccepted
    ) : IRequest<Guid>;
}
