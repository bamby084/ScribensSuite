using System.Threading.Tasks;
using PluginScribens.Common;

namespace PluginScribens.Common.SessionChecker
{
    public class Session
    {
        public static async Task NotifyScribensServer()
        {
            if (Globals.CurrentIdentity == null)
                return;

            ISessionChecker sessionChecker = new ScribensSessionChecker();
            await sessionChecker.NotifyAsync(Globals.CurrentIdentity.Email, Globals.Settings.Language.Abbreviation);
        }
    }
}
