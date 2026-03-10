using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Model
{
    public class Table : INotifyPropertyChanged
    {
        public string TableId { get; set; }
        public string TableName { get; set; }
        public int TableStatus { get; set; } // Trống: 0, Đang sử dụng: 1, Đang dọn dẹp: 2

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
