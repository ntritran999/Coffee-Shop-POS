using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Client.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
