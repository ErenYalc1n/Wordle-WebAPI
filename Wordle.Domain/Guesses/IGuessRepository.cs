namespace Wordle.Domain.Guesses
{
    public interface IGuessRepository
    {
        Task AddAsync(Guess guess);
        Task<List<Guess>> GetGuessesByUserAndDateAsync(Guid userId, DateTime date);
        Task<int> GetGuessCountForUserAsync(Guid userId, DateTime date);
    }
}
