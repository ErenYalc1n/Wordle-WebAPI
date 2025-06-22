namespace Wordle.Domain.User
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        void Update(User user); 
        void Delete(User user);
        Task<bool> IsReminderEmailAllowedAsync(Guid userId);       
        Task<bool> IsEmailConfirmedAsync(string email);
        Task<User?> AuthenticateAsync(string email, string passwordHash);
    }
}
