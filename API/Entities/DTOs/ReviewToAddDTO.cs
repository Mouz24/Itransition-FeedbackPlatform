using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReviewToAddDTO
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Review text is required")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Mark is required")]
        public int? Mark { get; set; }

        [Required]
        public string ArtworkName { get; set; }

        [Required]
        public int? GroupId { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
