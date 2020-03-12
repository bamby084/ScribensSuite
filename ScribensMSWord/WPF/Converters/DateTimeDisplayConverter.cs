using System;
using System.Globalization;
using System.Windows.Data;
using ScribensMSWord.Utils;

namespace ScribensMSWord.WPF.Converters
{
    public class DateTimeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format(new CultureInfo(Globals.Settings.Language.Culture), "{0:dd MMMM yyyy}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
