using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class Account
    {
        public string Username { get; set; }
        public string Password { get; set; } 
        public string DisplayName { get; set; }
        public string Role { get; set; } // Quản lý / Thu ngân
    }
}
