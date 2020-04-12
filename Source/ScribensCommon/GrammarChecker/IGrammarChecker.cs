using System.Threading.Tasks;

namespace PluginScribens.Common.GrammarChecker
{
    public interface IGrammarChecker
    {
        Task<GrammarSolutions> CheckAsync(string text, string language);
    }
}
