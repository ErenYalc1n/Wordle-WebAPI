using Microsoft.EntityFrameworkCore;
using Wordle.Domain.User;
using Wordle.Infrastructure.Data;

namespace Wordle.Infrastructure.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly WordleDbContext _context;

        public EfUserRepository(WordleDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User?> GetByIdAsync(Guid id) =>
            await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        public void Update(User user)
        {
            _context.Users.Update(user);
        }
        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
        public async Task<bool> IsEmailConfirmedAsync(string email) =>
            await _context.Users.AnyAsync(x => x.Email == email && x.IsEmailConfirmed);

        public async Task<bool> IsReminderEmailAllowedAsync(Guid userId) =>
            await _context.Users.AnyAsync(x => x.Id == userId && x.IsKvkkAccepted);

        public async Task<User?> AuthenticateAsync(string email, string passwordHash) =>
            await _context.Users.FirstOrDefaultAsync(x =>
                x.Email == email && x.PasswordHash == passwordHash);
    }

}
