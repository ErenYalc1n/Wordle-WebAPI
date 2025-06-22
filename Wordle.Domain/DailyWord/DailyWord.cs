using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle.Domain.DailyWord
{
    public sealed class DailyWord
    {
        public Guid Id { get; set; }
        public string Word { get; set; } = default!;
        public DateTime Date { get; set; }
    }
}
