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
            if (value == null) return "0 ₫";

            double amount = 0;
            if (value is int intValue) amount = intValue;
            else if (value is double doubleValue) amount = doubleValue;
            else if (value is float floatValue) amount = floatValue;
            else if (value is decimal decimalValue) amount = (double)decimalValue;
            else return "0 ₫";

            CultureInfo culture = CultureInfo.GetCultureInfo("vi-VN");
            var formatted = string.Format(culture, "{0:c0}", amount);
            return formatted;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
