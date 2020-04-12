using System.Threading.Tasks;

namespace PluginScribens.Common.IdentityChecker
{
    public interface IIdentityChecker
    {
        Task<Identity> CheckIdentityAsync(string username, string password, string language);
    }
}
