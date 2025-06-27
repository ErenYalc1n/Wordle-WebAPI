using Microsoft.EntityFrameworkCore;
using Wordle.Domain.Guesses;
using Wordle.Infrastructure.Data;

namespace Wordle.Infrastructure.Repositories;

public class EFGuessRepository : IGuessRepository
{
    private readonly WordleDbContext _context;

    public EFGuessRepository(WordleDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasCorrectGuessAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default)
    {
        return _context.Guesses
            .AnyAsync(g => g.UserId == userId && g.DailyWordId == dailyWordId && g.IsCorrect, cancellationToken);
    }

    public Task<int> GetGuessCountAsync(Guid userId, Guid dailyWordId, CancellationToken cancellationToken = default)
    {
        return _context.Guesses
            .CountAsync(g => g.UserId == userId && g.DailyWordId == dailyWordId, cancellationToken);
    }

    public async Task AddAsync(Guess guess, CancellationToken cancellationToken = default)
    {
        await _context.Guesses.AddAsync(guess, cancellationToken);
    }
}
