using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationContext applicationContext) :
            base(applicationContext) { }

        public void AddComment(Comment comment) // reviewID : TO-DO
        {
            comment.DateCreated = DateTime.UtcNow.AddHours(3);

            Create(comment);
        }

        public IEnumerable<Comment> GetAllComments(bool trackChanges) =>
            FindAll(trackChanges).
            OrderBy(c => c.DateCreated).
            ToList();

        public Comment GetComment(Guid id, bool trackChanges) =>
            FindByCondition(c => c.Id.Equals(id), trackChanges).FirstOrDefault();

        public IEnumerable<Comment> GetUserComments(Guid userId, bool trackChanges) =>
            FindByCondition(comment => comment.UserId.Equals(userId), trackChanges).ToList();

        public void RemoveComment(Guid id)
        {
            var comment = GetComment(id, false);

            Delete(comment);
        }

        public void RemoveComments(IEnumerable<Comment> comments)
        {
            DeleteAll(comments);
        }
    }
}
