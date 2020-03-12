using Newtonsoft.Json;
using System.Collections.Generic;

namespace ScribensMSWord.Checkers.GrammarChecker
{
    public class SolutionVector
    {
        public string Left { get; set; }
        public string Right { get; set; }
    }

    public class MotSolution
    {
        [JsonProperty("Start_Pos")]
        public int StartIndex { get; set; }

        [JsonProperty("End_Pos")]
        public int EndIndex { get; set; }

        public string IdPhrase { get; set; }

        public string Id { get; set; }

        public string IdFin { get; set; }

        public int Type { get; set; }

        [JsonProperty("vectSolution")]
        public List<SolutionVector> Vector { get; set; }

        public string ExplicationSolution { get; set; }

        [JsonProperty("EstSuggestion")]
        public bool Suggestion { get; set; }

        [JsonProperty("VectOtherResults")]
        public List<object> OtherResults { get; set; }
    }

    public class GrammarSolution
    {
        [JsonProperty("IndexPar")]
        public int ParagraphIndex { get; set; }

        [JsonProperty("LeftPos")]
        public int Left { get; set; }

        [JsonProperty("RightPos")]
        public int Right { get; set; }

        [JsonProperty("MotSolution")]
        public MotSolution Solution { get; set; }
    }

    public class MapPosSol
    {
        [JsonProperty("Cor")]
        public List<GrammarSolution> Corrections { get; set; }

        [JsonProperty("Redundancy")]
        public List<GrammarSolution> Redundancies { get; set; }

        [JsonProperty("pl")]
        public List<GrammarSolution> LongSentences { get; set; }

        [JsonProperty("Vocabulary_enhancement")]
        public List<GrammarSolution> VocabularyEnhancements { get; set; }

        [JsonProperty("Rephrase_inelegantforms")]
        public List<GrammarSolution> Reformulations { get; set; }

        [JsonProperty("Subjectivity_Positive")]
        public List<GrammarSolution> SubjectivityPositives { get; set; }

        [JsonProperty("Subjectivity_Pejorative")]
        public List<GrammarSolution> SubjectivityNegatives { get; set; }

        [JsonProperty("Rephrase_wordreducing")]
        public List<GrammarSolution> WordReductions { get; set; }

        public bool IsEmpty()
        {
            return Corrections == null || Corrections.Count == 0;
        }
    }

    public class GrammarCheckingResult
    {
        public string IdMax { get; set; }

        public int LimiteNbChar { get; set; }

        [JsonProperty("LimitCharExceeded_Trial")]
        public bool IsTrial { get; set; }

        [JsonProperty("Map_PosSol")]
        public MapPosSol Result { get; set; }
    }
}
