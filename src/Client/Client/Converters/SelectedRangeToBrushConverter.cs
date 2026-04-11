using Client.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Client.Converters
{
    public class SelectedRangeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeRange selected && parameter is string target)
            {
                if (Enum.TryParse<TimeRange>(target, out var t) && selected == t)
                    return new SolidColorBrush(Colors.Orange); // nút đang chọn
            }
            return new SolidColorBrush(Colors.White); // nút mặc định
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}