using Contracts;
using Entities.Models;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public void AddComment(Comment comment)
        {
            _commentRepository.AddComment(comment);
        }

        public IEnumerable<Comment> GetAllComments(bool trackChanges)
        {
            return _commentRepository.GetAllComments(trackChanges);
        }

        public Comment GetComment(Guid id, bool trackChanges)
        {
            return _commentRepository.GetComment(id, trackChanges);
        }

        public IEnumerable<Comment> GetUserComments(Guid userId, bool trackChanges)
        {
            return _commentRepository.GetUserComments(userId, trackChanges);
        }

        public void RemoveComment(Guid id)
        {
            _commentRepository.RemoveComment(id);
        }

        public void RemoveComments(IEnumerable<Comment> comments)
        {
            _commentRepository.RemoveComments(comments);
        }
    }
}
