using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Artwork
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rate { get; set; }

        public virtual ICollection<RatedArtwork> UsersRated { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }

    public class Review
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }

        [Range(1, 10, ErrorMessage = "Mark must be between 1 and 10.")]
        public int Mark { get; set; }

        public int Likes { get; set; }

        [NotMapped]
        public bool IsLikedByUser { get; set; }

        public virtual ICollection<ReviewImage> ReviewImages { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<LikedReview> UsersLiked { get; set; }
        public virtual ICollection<ReviewTag> Tags { get; set; }

        [ForeignKey("ArtworkId")]
        public Guid ArtworkId { get; set; }
        public virtual Artwork Artwork { get; set; }

        [ForeignKey("GroupId")]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("ReviewId")]
        public Guid ReviewId { get; set; }
        public virtual Review Review { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Text { get; set; }

        public virtual ICollection<ReviewTag> TaggedReview { get; set; }
    }

    public class LikedReview
    {
        public Guid Id { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("ReviewId")]
        public Guid ReviewId { get; set; }
        public virtual Review Review { get; set; }
    }

    public class RatedArtwork
    {
        public Guid Id { get; set; }

        public int Rate { get; set; }

        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("ArtworkId")]
        public Guid ArtworkId { get; set; }
        public virtual Artwork Artwork { get; set; }
    }

    public class ReviewImage
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey("ReviewId")]
        public Guid ReviewId { get; set; }
        public virtual Review Review { get; set; }
    }

    public class ReviewTag
    {
        public Guid Id { get; set; }

        [ForeignKey("ReviewId")]
        public Guid ReviewId { get; set; }
        public virtual Review Review { get; set; }

        [ForeignKey("TagId")]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
