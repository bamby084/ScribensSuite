
using System.Threading.Tasks;

namespace PluginScribens_Word.Checkers.IdentityChecker
{
    public interface IIdentityChecker
    {
        Task<Identity> CheckIdentityAsync(string username, string password, string language);
    }
}
