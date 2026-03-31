using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Client.Models
{
    public partial class BillItem : ObservableObject
    {
        public Product? Detail { get; set; }

        [ObservableProperty]
        public partial string Notes { get; set; } = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        public partial int Count { get; set; } = 0;

        public int TotalPrice => Detail!.Price * Count;

        [ObservableProperty]
        public partial bool IsNotRemoved { get; set; } = true;

        public BillItem(Product? product, int count)
        {
            this.Detail = product;
            this.Count = count;
        }

        [RelayCommand]
        public void RemoveItem()
        {
            IsNotRemoved = false;
        }
    }
}
