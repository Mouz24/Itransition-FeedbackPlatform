using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class AuthenticatedResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
    }
}
