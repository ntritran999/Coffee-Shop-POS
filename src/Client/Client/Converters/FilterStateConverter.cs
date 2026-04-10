using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;

namespace Client.Converters
{
    public class FilterStateConverter : IValueConverter
    {
        public string ActiveColorHex { get; set; } = "#D97724";
        public string InactiveColorHex { get; set; } = "#00000000"; // Mặc định trong suốt

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int selectedFilter && parameter != null)
            {
                if (int.TryParse(parameter.ToString(), out int targetFilter))
                {
                    string hex = selectedFilter == targetFilter ? ActiveColorHex : InactiveColorHex;
                    return GetSolidColorBrush(hex);
                }
            }
            return GetSolidColorBrush(InactiveColorHex);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();

        private SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", "");
            byte a = 255;
            byte r = 0, g = 0, b = 0;

            if (hex.Length == 8) // Có Alpha channel (VD: #00000000)
            {
                a = System.Convert.ToByte(hex.Substring(0, 2), 16);
                r = System.Convert.ToByte(hex.Substring(2, 2), 16);
                g = System.Convert.ToByte(hex.Substring(4, 2), 16);
                b = System.Convert.ToByte(hex.Substring(6, 2), 16);
            }
            else if (hex.Length == 6) // HEX thông thường
            {
                r = System.Convert.ToByte(hex.Substring(0, 2), 16);
                g = System.Convert.ToByte(hex.Substring(2, 2), 16);
                b = System.Convert.ToByte(hex.Substring(4, 2), 16);
            }

            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }
    }
}
