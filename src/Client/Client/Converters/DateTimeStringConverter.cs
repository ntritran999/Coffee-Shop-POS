using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Converters
{
    public class DateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dt)
            {
                return dt.ToString("dd/MM/yyyy hh:mm:ss tt");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
