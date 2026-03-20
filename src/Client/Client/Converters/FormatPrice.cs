using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Client.Converters
{
    public class FormatPrice : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 0;

            int amount = (int)value;
            CultureInfo culture = CultureInfo.GetCultureInfo("vi-VN");
            var formatted = string.Format(culture, "{0:c}", amount);
            return formatted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
