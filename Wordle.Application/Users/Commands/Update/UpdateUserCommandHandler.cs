using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Users;

namespace Wordle.Application.Users.Commands.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateUserCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            throw new Exception("Kimlik doğrulama başarısız.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new Exception("Kullanıcı bulunamadı.");

        user.Nickname = request.Nickname;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
