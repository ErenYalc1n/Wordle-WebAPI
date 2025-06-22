using MediatR;
using Wordle.Domain.User;

namespace Wordle.Application.User
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {           
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("Bu e-posta zaten kayıtlı.");
          
            var passwordHash = FakeHash(request.Password);
           
            var user = new Domain.User.User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                Nickname = request.Nickname,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = Role.Player,
                IsEmailConfirmed = false,
                IsKvkkAccepted = request.IsKvkkAccepted
            };

            await _userRepository.AddAsync(user);

            return user.Id;
        }
       
        private string FakeHash(string password)
        {
            return $"HASHED::{password}";
        }
    }
}
