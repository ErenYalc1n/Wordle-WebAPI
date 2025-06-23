using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Domain.Users;

namespace Wordle.Application.Users;

public class DeleteUserCommand : IRequest
{
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new Exception("Kullanıcı bulunamadı.");

        await _userRepository.DeleteAsync(user); // Bu metot henüz yoksa ekleyeceğiz

        return Unit.Value;
    }
}
