using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle.Domain.Guess
{
    public sealed class Guess
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }              
        public Guid DailyWordId { get; set; }    
        public string Word { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
