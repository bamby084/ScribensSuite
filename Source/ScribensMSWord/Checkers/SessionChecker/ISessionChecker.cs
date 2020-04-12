using Task = System.Threading.Tasks.Task;

namespace PluginScribens_Word.Checkers.SessionChecker
{
    public interface ISessionChecker
    {
        Task NotifyAsync(string userName, string language);
    }
}
