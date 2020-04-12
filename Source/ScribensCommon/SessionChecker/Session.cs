using System.Threading.Tasks;
using PluginScribens.Common;

namespace PluginScribens.Common.SessionChecker
{
    public class Session
    {
        public static async Task NotifyScribensServer()
        {
            if (Plugin.CurrentIdentity == null)
                return;

            ISessionChecker sessionChecker = new ScribensSessionChecker();
            await sessionChecker.NotifyAsync(Plugin.CurrentIdentity.Email, Plugin.Settings.Language.Abbreviation);
        }
    }
}
