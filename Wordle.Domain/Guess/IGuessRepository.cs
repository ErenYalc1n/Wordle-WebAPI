using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle.Domain.Guess
{
    public interface IGuessRepository
    {
        Task AddAsync(Guess guess);
        Task<List<Guess>> GetGuessesByUserAndDateAsync(Guid userId, DateTime date);
        Task<int> GetGuessCountForUserAsync(Guid userId, DateTime date);
    }
}
