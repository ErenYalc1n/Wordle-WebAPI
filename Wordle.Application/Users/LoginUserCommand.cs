using MediatR;

namespace Wordle.Application.Users.Commands
{
    public class LoginUserCommand : IRequest<LoginResultDto>
    {
        public string Identifier { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class LoginResultDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }

}
