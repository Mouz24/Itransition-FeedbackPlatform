using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ReviewForManipulationDTO
    {
        public string Title { get; set; }
        public string Text { get; set; }

        [Range(1, 10, ErrorMessage = "Mark must be between 1 and 10.")]
        public int Mark { get; set; }

        public int GroupId { get; set; }
        public ICollection<string> ImageUrls { get; set; } = new List<string>();
    }
}
