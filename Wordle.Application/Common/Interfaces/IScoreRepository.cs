namespace Wordle.Application.Common.Interfaces;

public interface IScoreRepository
{
    Task AddAsync(Score score);
    Task<bool> ExistsForUserAndWordAsync(Guid userId, Guid wordId);
}
