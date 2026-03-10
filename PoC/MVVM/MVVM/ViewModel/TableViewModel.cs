using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using MVVM.Model;

namespace MVVM.ViewModel
{
    public class TableViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Table> Tables { get; set; }

        public TableViewModel()
        {
            Tables = new ObservableCollection<Table>()
            {
                new Table{
                    TableId = "T01",
                    TableName = "Bàn 01",
                    TableStatus = 0
                },
                new Table{
                    TableId = "T02",
                    TableName = "Bàn 02",
                    TableStatus = 1
                },
                new Table{
                    TableId = "T03",
                    TableName = "Bàn 03",
                    TableStatus = 1
                },
                new Table{
                    TableId = "T04",
                    TableName = "Bàn 04",
                    TableStatus = 2
                },
                new Table{
                    TableId = "T05",
                    TableName = "Bàn 05",
                    TableStatus = 0
                },
                new Table{
                    TableId = "T06",
                    TableName = "Bàn 06",
                    TableStatus = 1
                },
                new Table{
                    TableId = "T07",
                    TableName = "Bàn 07",
                    TableStatus = 2
                },
                new Table{
                    TableId = "T08",
                    TableName = "Bàn 08",
                    TableStatus = 0
                }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
