namespace Post.Common.Events
{
    using CQRS.Core.Events;

    public class PostRemovedEvent : BaseEvent
    {
        public PostRemovedEvent() : base(nameof(PostRemovedEvent))
        {
        }
    }
}