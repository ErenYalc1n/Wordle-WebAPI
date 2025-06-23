namespace Wordle.Domain.Scores
{
    public sealed class Score
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public int Point { get; set; }
    }
}
