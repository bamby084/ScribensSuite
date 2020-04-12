using System.Collections.Generic;
using PluginScribens.Common;
using PluginScribens.Common.GrammarChecker;

namespace PluginScribens.UI.Messages
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
