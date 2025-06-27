using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Exceptions;
using Wordle.Domain.DailyWords;
using Wordle.Domain.Guesses;

namespace Wordle.Application.Guesses.Commands.Create;

public class CreateGuessCommandHandler : IRequestHandler<CreateGuessCommand, Guid>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGuessRepository _guessRepository;
    private readonly IDailyWordRepository _dailyWordRepository;

    public CreateGuessCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IGuessRepository guessRepository,
        IDailyWordRepository dailyWordRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _guessRepository = guessRepository;
        _dailyWordRepository = dailyWordRepository;
    }

    public async Task<Guid> Handle(CreateGuessCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanamadı.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var dailyWord = await _dailyWordRepository.GetByDateAsync(today);
        if (dailyWord is null)
            throw new NotFoundException("Bugünün kelimesi henüz belirlenmemiş.");

        if (await _guessRepository.HasCorrectGuessAsync(userId, dailyWord.Id, cancellationToken))
            throw new ConflictException("Zaten doğru tahmin yaptınız.");

        var guessCount = await _guessRepository.GetGuessCountAsync(userId, dailyWord.Id, cancellationToken);
        if (guessCount >= 5)
            throw new ConflictException("Tahmin hakkınız doldu.");

        var isCorrect = string.Equals(
            request.GuessText.Trim(),
            dailyWord.Word.Trim(),
            StringComparison.OrdinalIgnoreCase);

        var guess = new Guess
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DailyWordId = dailyWord.Id,
            GuessText = request.GuessText.Trim(),
            GuessedAt = DateTime.UtcNow,
            IsCorrect = isCorrect
        };

        await _guessRepository.AddAsync(guess, cancellationToken);

        return guess.Id;
    }
}
