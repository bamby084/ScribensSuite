﻿using System;
using System.Globalization;
using System.Windows.Data;
using PluginScribens.Common;

namespace PluginScribens.UI.Converters
{
    public class StringResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return Globals.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
