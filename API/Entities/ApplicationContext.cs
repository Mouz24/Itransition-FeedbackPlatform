using Entities.Configuration;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.Entity<User>()
            .HasMany(r => r.Comments)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
            .HasMany(r => r.LikedReviews)
            .WithOne(c => c.User)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("AspNetRoles");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        DbSet<Review> Reviews { get; set; }
        DbSet<Group> Groups { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Artwork> Artworks { get; set; }
        DbSet<LikedReview> LikedReviews { get; set; }
        DbSet<RatedArtwork> RatedArtworks { get; set; }
        DbSet<ReviewImage> ReviewImages { get; set; }
    }
}
