using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.DTOs;
using Wordle.Domain.Common;
using Wordle.Domain.DailyWords;
using Wordle.Domain.Guesses;

namespace Wordle.Application.Guesses.Commands.Create;

public class CreateGuessCommandHandler : IRequestHandler<CreateGuessCommand, GuessResponseDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGuessRepository _guessRepository;
    private readonly IDailyWordRepository _dailyWordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGuessCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IGuessRepository guessRepository,
        IDailyWordRepository dailyWordRepository,
        IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _guessRepository = guessRepository;
        _dailyWordRepository = dailyWordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GuessResponseDto> Handle(CreateGuessCommand request, CancellationToken cancellationToken)
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

        if (await _guessRepository.IsDuplicateGuessAsync(userId, dailyWord.Id, request.GuessText, cancellationToken))
            throw new ConflictException("Aynı tahmini tekrar gönderemezsiniz.");

        var normalizedGuess = request.GuessText.ToLowerInvariant();
        var normalizedTarget = dailyWord.Word.ToLowerInvariant();

        var result = new List<LetterResult>();
        var guessChars = normalizedGuess.ToCharArray();
        var targetChars = normalizedTarget.ToCharArray();
        var matched = new bool[5];

        for (int i = 0; i < 5; i++)
        {
            if (guessChars[i] == targetChars[i])
            {
                result.Add(new LetterResult { Letter = guessChars[i], Status = "green" });
                matched[i] = true;
            }
            else
            {
                result.Add(new LetterResult { Letter = guessChars[i], Status = "" });
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (result[i].Status == "green") continue;

            var found = false;
            for (int j = 0; j < 5; j++)
            {
                if (!matched[j] && guessChars[i] == targetChars[j])
                {
                    matched[j] = true;
                    found = true;
                    break;
                }
            }

            result[i].Status = found ? "yellow" : "gray";
        }

        var guess = new Guess
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DailyWordId = dailyWord.Id,
            GuessText = request.GuessText,
            GuessedAt = DateTime.UtcNow,
            IsCorrect = normalizedGuess == normalizedTarget
        };

        await _guessRepository.AddAsync(guess, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new GuessResponseDto
        {
            GuessId = guess.Id,
            GuessText = guess.GuessText,
            LetterResults = result,
            CurrentAttempt = guessCount + 1,
            RemainingAttempts = 5 - (guessCount + 1)
        };
    }
}
