namespace Wordle.Domain.Guesses
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
