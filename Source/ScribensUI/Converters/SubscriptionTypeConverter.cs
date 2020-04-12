using System;
using System.Globalization;
using System.Windows.Data;
using PluginScribens.Common;
using PluginScribens.Common.Enums;

namespace PluginScribens.UI.Converters
{
    public class SubscriptionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new NotSupportedException();

            var subscriptionType = (SubscriptionType) value;
            if (subscriptionType == SubscriptionType.NOT_AVAILABLE || subscriptionType == SubscriptionType.TRIAL)
                return Plugin.GetString("UserInfoPane.Subscription.NotExist");
            else if (subscriptionType == SubscriptionType.P1M || subscriptionType == SubscriptionType.P1M_RA)
                return $"{Plugin.GetString("UserInfoPane.SubscriptionType_P1M")}";
            else if (subscriptionType == SubscriptionType.P3M || subscriptionType == SubscriptionType.P3M_RA)
                return $"{Plugin.GetString("UserInfoPane.SubscriptionType_P3M")}";
            else if (subscriptionType == SubscriptionType.P1A || subscriptionType == SubscriptionType.P1A_RA)
                return $"{Plugin.GetString("UserInfoPane.SubscriptionType_P1A")}";
            else if (subscriptionType.ToString().EndsWith("_EXPIRED"))
                return Plugin.GetString("UserInfoPane.Subscription.Expired");

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
