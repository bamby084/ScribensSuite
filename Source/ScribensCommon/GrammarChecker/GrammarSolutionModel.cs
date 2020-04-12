using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using PluginScribens.Common.Enums;


namespace PluginScribens.Common.GrammarChecker
{
    public class GrammarSolutionModel : IDisposable, INotifyPropertyChanged, IEquatable<GrammarSolutionModel>
    {
        #region Properties
        public event EventHandler OnCorrected;
        
        // Color for correction highlight
        public static WdColor CorrectionHighlightColor = ColorHelper.ConvertHexColorToWordColor("#fdcac0");

        // Color for style highlight
        public static WdColor StyleHighlightColor = ColorHelper.ConvertHexColorToWordColor("#fffa84");

        // Mode of the solution
        public SolutionMode Mode { get; set; }
        // Paragraph index of the solution
        public int ParagraphIndex { get; set; }
        // Start index of the solution in the paragraph
        public int Start { get; set; }
        // End index of the solution in the paragraph
        public int End { get; set; }
        // Word of the solution
        private string _originalWord;
        public string OriginalWord {
            get => _originalWord;
            set
            {
                _originalWord = value;
                if (Suggestions != null)
                {
                    foreach (var suggestion in Suggestions)
                    {
                        suggestion.Original = _originalWord;
                    }
                }
            }
        }

        // Lkist of the propositions
        public List<GrammarSuggestion> Suggestions { get; set; }
        // Explanation of the correction
        public string Explanation { get; set; }
        // Range of the solution
        public Range Range { get; set; }
        // Range of the paragraph containing the solution
        public Range RangeP = null;
        // Index of the start range
        public int Index_StartRange = -1;
        // If the solution is highlighted
        public bool Highlighted = false;
        #endregion

        #region ctors
        public GrammarSolutionModel()
        {
            Suggestions = new List<GrammarSuggestion>();
        }
        #endregion

        #region Methods
        // Highlight the solution
        public void HighLight()
        {
            if (Range == null)
                return;

            try
            {
                Range.Font.Shading.BackgroundPatternColor = Mode == SolutionMode.Correction ? CorrectionHighlightColor : StyleHighlightColor;
                Highlighted = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        // Remove the highlight of the solution
        public void ClearHighLight()
        {
            if (Range == null)
                return;

            try
            {
                var backgroundPatternColor = Range.Font.Shading.BackgroundPatternColor;
                if (backgroundPatternColor == CorrectionHighlightColor ||
                    backgroundPatternColor == StyleHighlightColor)
                {
                    Range.Font.Shading.BackgroundPatternColor = WdColor.wdColorAutomatic;
                    Highlighted = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        
        // Add a proposition
        public void AddSuggestion(GrammarSuggestion suggestion)
        {
            suggestion.OnApplySuggestion += OnApplyCorrection;
            Suggestions.Add(suggestion);
        }

        public void Dispose()
        {
            if (Range != null)
            {
                ClearHighLight();
                Marshal.ReleaseComObject(Range);
                Range = null;
            }
        }

        public void ApplyDefaultSuggestion()
        {
            try
            {
                ClearHighLight();
                if (Suggestions.Any())
                {
                    this.Range.Text = Suggestions[0].Suggestion;
                }
                OnCorrected?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        // Apply the correction
        private void OnApplyCorrection(object sender, ApplySuggestionEventHandlerArgs e)
        {
            try
            {
                ClearHighLight();
                this.Range.Text = e.Suggestion;
                OnCorrected?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IEquatable
        // Equality operator
        public bool Equals(GrammarSolutionModel other)
        {
            return this.Start == other.Start && this.End == other.End && this.ParagraphIndex == other.ParagraphIndex && this.Mode == other.Mode;
        }

        public override bool Equals(object obj)
        {
            return Equals((GrammarSolutionModel)obj);
        }

        public override int GetHashCode()
        {
            return this.Start.GetHashCode() ^ this.End.GetHashCode() ^ this.Mode.GetHashCode();
        }
        #endregion
    }
}
