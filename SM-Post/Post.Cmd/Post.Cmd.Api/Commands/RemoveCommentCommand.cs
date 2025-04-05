using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{

    public class RemoveCommentCommand : BaseCommand
    {
        public string CommentId { get; set; }
        public string Username { get; set; }
    }
}