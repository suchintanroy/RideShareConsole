using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RideshareConsole.Enum;

namespace RideshareConsole.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserId { get; set; }  // Company user ID
        public string Name { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}