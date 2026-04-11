using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Converters
{
    public class FormatTableStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            
            int status = (int)value;
            return status switch
            {
                0 => "Trống",
                1 => "Có người",
                2 => "Đang dọn",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
