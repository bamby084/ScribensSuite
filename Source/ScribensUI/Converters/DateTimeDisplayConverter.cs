using System;
using System.Globalization;
using System.Windows.Data;
using PluginScribens.Common;

namespace PluginScribens.UI.Converters
{
    public class DateTimeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(new CultureInfo(Plugin.Settings.Language.Culture), "{0:dd MMMM yyyy}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
