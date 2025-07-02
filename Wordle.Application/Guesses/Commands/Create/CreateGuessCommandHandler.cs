using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Wordle.Application.Common.Exceptions;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.DTOs;
using Wordle.Domain.Common;
using Wordle.Domain.Guesses;

namespace Wordle.Application.Guesses.Commands.Create;

public class CreateGuessCommandHandler : IRequestHandler<CreateGuessCommand, GuessResponseDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGuessRepository _guessRepository;
    private readonly IDailyWordRepository _dailyWordRepository;
    private readonly IScoreRepository _scoreRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateGuessCommandHandler> _logger;

    public CreateGuessCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IGuessRepository guessRepository,
        IDailyWordRepository dailyWordRepository,
        IScoreRepository scoreRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateGuessCommandHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _guessRepository = guessRepository;
        _dailyWordRepository = dailyWordRepository;
        _scoreRepository = scoreRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GuessResponseDto> Handle(CreateGuessCommand request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            _logger.LogWarning("Guess: Kullanıcı kimliği doğrulanamadı.");
            throw new UnauthorizedAppException("Kullanıcı kimliği doğrulanamadı.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dailyWord = await _dailyWordRepository.GetByDateAsync(today);
        if (dailyWord is null)
        {
            _logger.LogWarning("Guess: Bugünün kelimesi bulunamadı.");
            throw new NotFoundException("Bugünün kelimesi henüz belirlenmemiş.");
        }

        if (await _guessRepository.HasCorrectGuessAsync(userId, dailyWord.Id, cancellationToken))
        {
            _logger.LogWarning("Guess: Kullanıcı zaten doğru tahmin yaptı. UserId: {UserId}", userId);
            throw new ConflictException("Zaten doğru tahmin yaptınız.");
        }

        var guessCount = await _guessRepository.GetGuessCountAsync(userId, dailyWord.Id, cancellationToken);
        if (guessCount >= 5)
        {
            _logger.LogWarning("Guess: Tahmin hakkı doldu. UserId: {UserId}", userId);
            throw new ConflictException("Tahmin hakkınız doldu.");
        }

        if (await _guessRepository.IsDuplicateGuessAsync(userId, dailyWord.Id, request.GuessText, cancellationToken))
        {
            _logger.LogWarning("Guess: Kullanıcı aynı tahmini tekrar gönderdi. UserId: {UserId}, Text: {GuessText}", userId, request.GuessText);
            throw new ConflictException("Aynı tahmini tekrar gönderemezsiniz.");
        }

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

        if (guess.IsCorrect)
        {
            var scoreExists = await _scoreRepository.ExistsForUserAndWordAsync(userId, dailyWord.Id);
            if (!scoreExists)
            {
                var scorePoint = guessCount switch
                {
                    0 => 10,
                    1 => 5,
                    2 => 4,
                    3 => 3,
                    4 => 2,
                    _ => 0
                };

                var score = new Score
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DailyWordId = dailyWord.Id,
                    Date = today,
                    Point = scorePoint
                };

                await _scoreRepository.AddAsync(score);
                _logger.LogInformation("Guess: Doğru tahmin sonrası skor eklendi. UserId: {UserId}, Puan: {Point}", userId, scorePoint);
            }
        }
        else if (guessCount == 4)
        {
            var scoreExists = await _scoreRepository.ExistsForUserAndWordAsync(userId, dailyWord.Id);
            if (!scoreExists)
            {
                var score = new Score
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DailyWordId = dailyWord.Id,
                    Date = today,
                    Point = 0
                };

                await _scoreRepository.AddAsync(score);
                _logger.LogInformation("Guess: Tahminler başarısız oldu, sıfır puan eklendi. UserId: {UserId}", userId);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var previousGuesses = await _guessRepository
            .GetGuessesForUserAndWordAsync(userId, dailyWord.Id, cancellationToken);

        var previousGuessDtos = previousGuesses
            .OrderBy(g => g.GuessedAt)
            .Select(g => new PreviousGuessDto
            {
                GuessText = g.GuessText,
                GuessedAt = g.GuessedAt
            }).ToList();

        return new GuessResponseDto
        {
            GuessId = guess.Id,
            GuessText = guess.GuessText,
            LetterResults = result,
            CurrentAttempt = guessCount + 1,
            RemainingAttempts = 5 - (guessCount + 1),
            IsCorrect = guess.IsCorrect,
            PreviousGuesses = previousGuessDtos
        };
    }
}
