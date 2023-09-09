using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class CommentToLeaveDTO
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public Guid ReviewId { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
