namespace Wordle.Domain.Guesses;

public interface IGuessRepository
{
    Task<bool> HasCorrectGuessAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default);
    Task<int> GetGuessCountAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default);
    Task AddAsync(Guess guess, CancellationToken cancellationToken = default);
}
