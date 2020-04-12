using System;
using System.Globalization;
using System.Windows.Data;
using PluginScribens.Common;
using PluginScribens.Common.ExtensionMethods;
using PluginScribens.Common.IdentityChecker;

namespace PluginScribens.UI.Converters
{
    public class RemainingTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var identity = value as Identity;
            if (identity == null)
                return null;

            if (identity.IsExpired())
                return Globals.GetString("UserInfoPane.Subscription.Expired");

            DateTime now = DateTime.Now;
            DateTime expired = identity.ExpiredDate.Value.MaxTimeOfDay();

            int totalMonths = (expired.Year - now.Year) * 12 + expired.Month - now.Month;
            if(totalMonths >= 12)
            {
                int totalYears = totalMonths / 12 + (totalMonths % 12 > 0 ? 1 : 0);
                return Pluralize(totalYears, Globals.GetString("UserInfoPane.Message.RemainingYears"));
            }

            if (totalMonths > 0)
            {
                return Pluralize(totalMonths, Globals.GetString("UserInfoPane.Message.RemainingMonths"));
            }

            int totalDays = (int)Math.Ceiling((expired - now).TotalDays);
            if (totalDays > 0)
                return Pluralize(totalDays, Globals.GetString("UserInfoPane.Message.RemainingDays"));

            return Globals.GetString("UserInfoPane.Subscription.Expired");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string Pluralize(int number, string phrase)
        {
            var pluralizationService = PluralizationService.Create(Globals.CurrentCulture);
            if (number > 1)
                return $"{number} {pluralizationService.Pluralize(phrase)}";

            return $"{number} {phrase}";
        }
    }
}
