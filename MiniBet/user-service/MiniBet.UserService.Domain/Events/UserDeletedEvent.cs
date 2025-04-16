namespace MiniBet.UserService.Domain.Events;

public record class UserDeletedEvent : UserEvent
{
    // UserId from base class is already inherited
}
