using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public int Mark { get; set; }
        public int Likes { get; set; }
        public bool IsLikedByUser { get; set; }
        public ArtworkDTO Artwork { get; set; }
        public GroupDTO Group { get; set; }
        public UserDTO User { get; set; }
        public ICollection<ReviewTagDTO> Tags { get; set; }
        public ICollection<ReviewImageDTO> ReviewImages { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
    }

    public class ArtworkDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }
    }

    public class GroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public int Likes { get; set; }
    }

    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public UserDTO User { get; set; }
    }

    public class ReviewImageDTO
    {
        public string imageUrl { get; set; }
    }

    public class ReviewTagDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int Count { get; set; }
    }
}
