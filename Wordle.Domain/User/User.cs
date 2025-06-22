namespace Wordle.Domain.User
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Nickname { get; set; } = default!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Role Role { get; set; } = Role.Player;
        public bool IsEmailConfirmed { get; set; }
        public bool IsKvkkAccepted { get; set; }
    }
}
