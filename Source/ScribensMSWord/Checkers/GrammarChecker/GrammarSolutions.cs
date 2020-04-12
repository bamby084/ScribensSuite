using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PluginScribens_Word.Enums;

namespace PluginScribens_Word.Checkers.GrammarChecker
{
    public class GrammarSolutions: Collection<GrammarSolutionModel>
    {
        public int LimiteNbChar { get; set; } = -1;

        public bool IsTrial { get; set; }

        public void Add(IEnumerable<GrammarSolutionModel> solutions)
        {
            foreach (var solution in solutions)
                this.Add(solution);
        }

        public IEnumerable<GrammarSolutionModel> GetSolutions(SolutionMode mode)
        {
            return this.Where(s => s.Mode == mode);
        }

        public void Remove(SolutionMode mode)
        {
            var itemsToRemove = this.Where(item => item.Mode == mode).ToList();
            foreach (var item in itemsToRemove)
            {
                this.Remove(item);
            }
        }
    }
}
