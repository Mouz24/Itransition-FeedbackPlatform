using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class UserForRegistrationDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression(@"^(?=.*[a-zA-Z]).+$", ErrorMessage = "Username must contain at least one letter")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public ICollection<string> Role { get; set; }
    }
}
