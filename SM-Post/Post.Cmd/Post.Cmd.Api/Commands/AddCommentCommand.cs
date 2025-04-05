namespace Post.Cmd.Api.Commands
{
    using CQRS.Core.Commands;

    public class AddCommentCommand : BaseCommand
    {
        public string Comment { get; set; }

        public string Username { get; set; }
    }
}