
using System.Threading.Tasks;

namespace ScribensMSWord.Checkers.IdentityChecker
{
    public interface IIdentityChecker
    {
        Task<Identity> CheckIdentityAsync(string username, string password, string language);
    }
}
