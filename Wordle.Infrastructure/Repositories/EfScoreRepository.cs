using Microsoft.EntityFrameworkCore;
using Wordle.Application.Common.Interfaces;
using Wordle.Infrastructure.Data;

public class EfScoreRepository : IScoreRepository
{
    private readonly WordleDbContext _context;

    public EfScoreRepository(WordleDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Score score)
    {
        await _context.Scores.AddAsync(score);
    }

    public async Task<bool> ExistsForUserAndWordAsync(Guid userId, Guid wordId)
    {
        return await _context.Scores.AnyAsync(s => s.UserId == userId && s.DailyWordId == wordId);
    }
}
