using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ExternalAuthDTO
    {
        public string Provider { get; set; }
        public string IdToken { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
    }
}
