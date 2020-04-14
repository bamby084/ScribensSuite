using PluginScribens.Common.Properties;
using System.Globalization;
using PluginScribens.Common.IdentityChecker;

namespace PluginScribens.Common
{
    public delegate void GlobalEventHandler(object sender, string eventName);

    public class Plugin
    {
        public static event GlobalEventHandler GlobalEvent;

        private static readonly object LockObject = new object();

        public static string GetString(string name)
        {
            return Strings.ResourceManager.GetString(name, Strings.Culture);
        }

        private static Settings _settings;
        public static Settings Settings
        {
            get
            {
                lock(LockObject)
                {
                    if(_settings == null)
                    {
                        _settings = new Settings();
                        _settings.Load();
                    }

                    return _settings;
                }
            }
        }

        public static Identity CurrentIdentity { get; set; }

        public static CultureInfo CurrentCulture { get; set; } = new CultureInfo("fr-FR");

        public static IWindowManager Windows { get; set; }

        public static void SetResourcesCulture(CultureInfo culture)
        {
            Strings.Culture = culture;
        }

        public static void PublishEvent(string eventName)
        {
            GlobalEvent?.Invoke(null, eventName);
        }
    }

}
