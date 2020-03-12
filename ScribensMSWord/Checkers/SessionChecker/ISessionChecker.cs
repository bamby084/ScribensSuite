using Task = System.Threading.Tasks.Task;

namespace ScribensMSWord.Checkers.SessionChecker
{
    public interface ISessionChecker
    {
        Task NotifyAsync(string userName, string language);
    }
}
