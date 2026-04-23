using Client.ViewModels;
using Microsoft.UI.Xaml.Data;
using System;

namespace Client.Converters
{
    public class FirstImageLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var raw = value?.ToString();
            var links = ProductViewModel.SplitImageLinks(raw);
            return links.Count > 0 ? links[0] : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
