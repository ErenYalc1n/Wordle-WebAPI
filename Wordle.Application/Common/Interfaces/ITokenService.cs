using Wordle.Domain.Users;

namespace Wordle.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }

}
