using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public static class SessionManager
    {
        public static Account? CurrentAccount { get; set; }

        public static bool IsAdmin => CurrentAccount?.Role == "Manager";
    }
}
