using PluginScribens_Word.Checkers.SessionChecker;
using Task = System.Threading.Tasks.Task;

namespace PluginScribens_Word.Utils
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
