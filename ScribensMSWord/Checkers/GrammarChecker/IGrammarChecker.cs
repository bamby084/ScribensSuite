using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScribensMSWord.Checkers.GrammarChecker
{
    public interface IGrammarChecker
    {
        Task<GrammarSolutions> CheckAsync(string text, string language);
    }
}
