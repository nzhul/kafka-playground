namespace MiniBet.UserService.Infrastructure.Data.Entities;

public class User
{
    public required Guid Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }
}
