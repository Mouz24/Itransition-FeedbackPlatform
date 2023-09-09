using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        IEnumerable<Comment> GetAllComments(bool trackChanges);
        void RemoveComment(Guid id);
        void RemoveComments(IEnumerable<Comment> comments);
        Comment GetComment(Guid id, bool trackChanges);
        IEnumerable<Comment> GetUserComments(Guid userId, bool trackChanges);
    }
}
