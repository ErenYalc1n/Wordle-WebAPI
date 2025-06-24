using MediatR;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Users.Commands.Register;
using Wordle.Application.Users.DTOs;
using Wordle.Domain.Users;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEMailService _emailService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEMailService emailService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<AuthResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Bu e-posta zaten kayıtlı.");

        var passwordHash = FakeHash(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Nickname = request.Nickname,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = Role.UnverifiedPlayer,
            IsKvkkAccepted = request.IsKvkkAccepted,
            IsEmailConfirmed = false,
            EmailVerificationCode = new Random().Next(100000, 999999).ToString(),
            EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        await _userRepository.AddAsync(user);

        await _emailService.SendEmailAsync(
            user.Email,
            "Wordle Mail Adresi Onayı",
            $"Doğrulama Kodunuz: {user.EmailVerificationCode}");

        var tokens = _tokenService.CreateToken(user);
        user.RefreshToken = tokens.RefreshToken;
        user.RefreshTokenExpiresAt = tokens.RefreshTokenExpiresAt;

        await _userRepository.UpdateAsync(user);

        return new AuthResultDto
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }

    private string FakeHash(string password) => $"HASHED::{password}";
}

