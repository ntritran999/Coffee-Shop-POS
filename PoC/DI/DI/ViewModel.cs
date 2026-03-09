using DI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI
{
    public class ViewModel
    {
        public Model MyModel { get; set; }
        private IMessageService _messageService;

        public ViewModel(IMessageService messageService) {
            _messageService = messageService;

            MyModel = new Model
            {
                Name = "Alice",
            };
        }

        public String ShowMessage(String Name)
        {
            return _messageService.ShowMessage(Name);
        }
    }
}
