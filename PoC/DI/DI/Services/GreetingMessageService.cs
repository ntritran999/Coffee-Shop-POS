using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Services
{
    public class GreetingMessageService : IMessageService
    {
        public String ShowMessage(String Name)
        {
            // Perform service
            Debug.WriteLine("Performing a service...");

            String result = $"Hello, {Name}!";
            return result;
        }
    }
}
