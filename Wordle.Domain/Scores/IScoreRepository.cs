namespace Wordle.Domain.Scores
{
    public interface IScoreRepository
    {
        Task AddAsync(Score score);      
        Task<List<Score>> GetScoresBetweenAsync(DateTime start, DateTime end);     
        Task<int> GetUserScoreTotalAsync(Guid userId, DateTime start, DateTime end);       
        Task<List<(string Nickname, int Score)>> GetTopScoresAsync(DateTime start, DateTime end, int top);       
        Task<int?> GetUserRankAsync(Guid userId, DateTime start, DateTime end);
    }
}
