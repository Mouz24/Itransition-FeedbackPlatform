using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class User : IdentityUser<Guid>
    {
        public int Likes { get; set; }
        public string? Avatar { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool isBlocked { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<LikedReview> LikedReviews { get; set; }
        public virtual ICollection<RatedArtwork> RatedArtworks { get; set; }
    }
}
