using Microsoft.EntityFrameworkCore;
using Wordle.Domain.Users;
using Wordle.Domain.DailyWords;
using Wordle.Domain.Guesses;

namespace Wordle.Infrastructure.Data
{
    public class WordleDbContext : DbContext
    {
        public WordleDbContext(DbContextOptions<WordleDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<DailyWord> DailyWords => Set<DailyWord>();
        public DbSet<Guess> Guesses => Set<Guess>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WordleDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
