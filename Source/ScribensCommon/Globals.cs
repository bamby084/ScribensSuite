using PluginScribens.Common.Properties;
using System.Globalization;
using PluginScribens.Common.IdentityChecker;

namespace PluginScribens.Common
{
    public partial class Globals
    {
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
    }
}
