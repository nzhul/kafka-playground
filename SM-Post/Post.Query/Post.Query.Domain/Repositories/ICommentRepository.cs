using System;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task CreateAsync(CommentEntity commentEntity);

        Task UpdateAsync(CommentEntity commentEntity);

        Task<CommentEntity> GetByIdAsync(Guid commentId);

        Task DeleteAsync(Guid commentId);
    }
}