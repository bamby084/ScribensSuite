using System.Collections.Generic;
using PluginScribens_Word.Checkers.GrammarChecker;
using PluginScribens_Word.ExtensionMethods;

namespace PluginScribens_Word.WPF.Messages
{
    public class CheckingCompletedMessage: ViewModelMessage
    {
        public GrammarSolutions Solutions { get; set; }
        public List<ParagraphInfo> ParagraphIndices { get; set; }
        // Index of indice paragraph of removing. Only when removing paragraph change.
        public int IndPSupp = -1;
        // Difference of number of paragraph counter in the change.
        public int DiffNbPar = -1;
    }
}
