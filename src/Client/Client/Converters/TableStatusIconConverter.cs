using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Converters
{
    public class TableStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";
            
            int status = (int)value;

            return status switch
            {
                0 => "\uE930",
                1 => "\uE716",
                2 => "\uE895",
                _ => ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
