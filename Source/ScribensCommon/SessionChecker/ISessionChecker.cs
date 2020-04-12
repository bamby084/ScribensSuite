using System.Threading.Tasks;

namespace PluginScribens.Common.SessionChecker
{
    public interface ISessionChecker
    {
        Task NotifyAsync(string userName, string language);
    }
}
