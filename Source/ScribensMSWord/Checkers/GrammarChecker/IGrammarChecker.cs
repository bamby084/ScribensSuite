using System.Collections.Generic;
using System.Threading.Tasks;

namespace PluginScribens_Word.Checkers.GrammarChecker
{
    public interface IGrammarChecker
    {
        Task<GrammarSolutions> CheckAsync(string text, string language);
    }
}
