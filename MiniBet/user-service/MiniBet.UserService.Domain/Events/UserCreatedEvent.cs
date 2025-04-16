namespace MiniBet.UserService.Domain.Events;

public record UserCreatedEvent : UserEvent
{
    public required string Username { get; set; }
    
    public required string Email { get; set; }
}