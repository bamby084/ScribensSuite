using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ScribensMSWord.Checkers.GrammarChecker;
using ScribensMSWord.Enums;

namespace ScribensMSWord.ExtensionMethods
{
    public static class GrammarInfoExtensions
    {
        public static GrammarSolutions ToGrammarSolutions(this GrammarCheckingResult grammarCheckingResult)
        {
            var solutions = new GrammarSolutions();
            solutions.Add(grammarCheckingResult.Result.Corrections.ToGrammarSolutionModels(SolutionMode.Correction));
            solutions.Add(grammarCheckingResult.Result.Redundancies.ToGrammarSolutionModels(SolutionMode.Redundancy));
            solutions.Add(grammarCheckingResult.Result.LongSentences.ToGrammarSolutionModels(SolutionMode.LongSentence));
            solutions.Add(grammarCheckingResult.Result.Reformulations.ToGrammarSolutionModels(SolutionMode.Reformulation));
            solutions.Add(grammarCheckingResult.Result.SubjectivityNegatives.ToGrammarSolutionModels(SolutionMode.SubjectivityNegative));
            solutions.Add(grammarCheckingResult.Result.SubjectivityPositives.ToGrammarSolutionModels(SolutionMode.SubjectivityPositive));
            solutions.Add(grammarCheckingResult.Result.VocabularyEnhancements.ToGrammarSolutionModels(SolutionMode.VocabularyEnhancement));
            solutions.Add(grammarCheckingResult.Result.WordReductions.ToGrammarSolutionModels(SolutionMode.WordReduction));

            solutions.LimiteNbChar = grammarCheckingResult.LimiteNbChar;
            solutions.IsTrial = grammarCheckingResult.IsTrial;

            return solutions;
        }

        public static IEnumerable<GrammarSolutionModel> ToGrammarSolutionModels(this IEnumerable<GrammarSolution> solutions, SolutionMode mode)
        {
            if (solutions == null)
                return new List<GrammarSolutionModel>();

            var models = new List<GrammarSolutionModel>();
            foreach (var solution in solutions)
            {
                models.Add(solution.ToGrammarSolutionModel(mode));
            }

            return models.OrderBy(s => s.ParagraphIndex).ThenBy(s => s.Start);
        }

        public static GrammarSolutionModel ToGrammarSolutionModel(this GrammarSolution solution, SolutionMode mode)
        {
            var model = new GrammarSolutionModel()
            {
                ParagraphIndex = solution.ParagraphIndex + 1,//+1 because VSTO uses 1-based index for paragraph while Scribens uses 0-based index
                Start = solution.Left,
                End = solution.Right,
                Explanation = StripOutHyperLink(solution.Solution.ExplicationSolution),
                Mode = mode
            };

            foreach (var vector in solution.Solution.Vector)
            {
                model.AddSuggestion(new GrammarSuggestion(vector.Left));
            }

            return model;
        }

        private static string StripOutHyperLink(string text)
        {
            if (text.IsNull())
                return null;

            text = text.Replace("</p>", "</p><br><br>");
            string pattern = "<a(.*?)>(.*?)</a>";
            var match = Regex.Match(text, pattern);
            if (match.Success)
            {
                text = text.Replace(match.Value, "");
                text += $"{match.Value.Replace("Règle générale", "Règle générale »")}";

                return text;
            }

            return text;
        }
    }
}
