using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class GroupToAddDTO
    {
        [Required(ErrorMessage = "Group name is required")]
        public string Name { get; set; }
    }
}
