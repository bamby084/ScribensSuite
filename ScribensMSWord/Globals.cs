using System.Globalization;
using ScribensMSWord.Checkers.IdentityChecker;
using ScribensMSWord.Properties;
using ScribensMSWord.Utils;
using Settings = ScribensMSWord.Utils.Settings;

namespace ScribensMSWord
{
    internal partial class Globals
    {
        private static readonly object _lockObject = new object();

        public static bool BackStageVisible { get; set; }

        private static WindowManager _windows = new WindowManager();
        public static WindowManager Windows { get; } = _windows;

        public static string GetString(string name)
        {
            return Strings.ResourceManager.GetString(name, Strings.Culture);
        }

        public static Identity CurrentIdentity { get; set; }

        private static Utils.Settings _settings;
        public static Utils.Settings Settings
        {
            get
            {
                lock(_lockObject)
                {
                    if(_settings == null)
                    {
                        _settings = new Utils.Settings();
                        _settings.Load();
                    }

                    return _settings;
                }
            }
        }

        public static CultureInfo CurrentCulture { get; set; } = new CultureInfo("fr-FR");
    }
}
