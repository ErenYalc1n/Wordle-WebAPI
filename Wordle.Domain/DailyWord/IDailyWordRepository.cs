using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle.Domain.DailyWord
{
    public interface IDailyWordRepository
    {
        Task<DailyWord?> GetTodayWordAsync();
        Task<List<DailyWord>> GetPastWordsAsync(int page, int pageSize);
        Task<List<DailyWord>> GetPlannedWordsAsync(int page, int pageSize);
        Task AddAsync(DailyWord word);
        Task<List<DailyWord>> SearchAsync(string keyword, bool isPast, int page, int pageSize);
        Task<bool> IsDateTakenAsync(DateTime date);
    }
}
