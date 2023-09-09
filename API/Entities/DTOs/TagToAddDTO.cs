using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class TagToAddDTO
    {
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
    }
}
