using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Converters
{
    public class BillStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";

            int status = (int)value;

            return status switch
            {
                1 => "\uE890",
                _ => "\uE70F",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
