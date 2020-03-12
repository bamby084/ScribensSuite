using Microsoft.Office.Interop.Word;
using ScribensMSWord.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using ScribensMSWord.Checkers.GrammarChecker;
using ScribensMSWord.Enums;
using ScribensMSWord.Utils;
using ScribensMSWord.WPF.Messages;

namespace ScribensMSWord.WPF.ViewModels
{
    public class SolutionsViewModel: BaseViewModel,
        IMessageHandler<CheckingStartsMessage>, 
        IMessageHandler<CheckingCompletedMessage>,
        IMessageHandler<LimCharExceededMessage>,
        IMessageHandler<ShowErrorMessage>
    {
        #region ctors
        public SolutionsViewModel()
        {
            InitializeCommands();
            UpdateLocalizationResources();
        }
        #endregion

        #region Properties
        // Current document of the current window
        public Document AssociatedDocument { get; set; }
        // Solutuons
        public GrammarSolutions Solutions { get; set; }
        // Thread counter. If a thread of scrollinbar stabilized start, the others stop.
        public static int Cnt_Thread = 0;

        // Solutions of the current mode
        private ObservableCollection<GrammarSolutionModel> _activeSolutions;
        public ObservableCollection<GrammarSolutionModel> ActiveSolutions
        {
            get => _activeSolutions;
            set
            {
                if (value != _activeSolutions)
                {
                    _activeSolutions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private GrammarSolutionModel _selectedSolution;
        public GrammarSolutionModel SelectedSolution
        {
            get => _selectedSolution;
            set
            {
                if (value != _selectedSolution)
                {
                    _selectedSolution = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(NoSuggestionVisibility));
                }
            }
        }

        // Current mode
        private SolutionMode _activeMode = SolutionMode.Correction;
        public SolutionMode ActiveMode
        {
            get => _activeMode;
            set
            {
                if (value != _activeMode)
                {
                    _activeMode = value;
                    
                    NotifyPropertyChanged();

                    // UnHighlight all active solutions
                    ClearHighLights();
                    // Clear active solutions
                    ClearActiveSolutions();
                    // Set active solutions of the new mode
                    SetActiveSolutions();
                    // Show solutions of the new mode
                    DisplayActiveSolutions();
                    // Update status
                    UpdateStatus();
                }
            }
        }

        public Visibility NoSuggestionVisibility
        {
            get
            {
                if (_selectedSolution != null && _selectedSolution.Suggestions.Count == 0)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        private Visibility _noIssueImageVisiblity = Visibility.Collapsed;
        public Visibility NoIssueImageVisiblity
        {
            get => _noIssueImageVisiblity;
            set
            {
                if (value != _noIssueImageVisiblity)
                {
                    _noIssueImageVisiblity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility _loadingImageVisibility = Visibility.Collapsed;
        public Visibility LoadingImageVisibility
        {
            get => _loadingImageVisibility;
            set
            {
                if (value != _loadingImageVisibility)
                {
                    _loadingImageVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility _issueCountControlVisibility = Visibility.Collapsed;
        public Visibility IssueCountControlVisibility
        {
            get => _issueCountControlVisibility;
            set
            {
                if (value != _issueCountControlVisibility)
                {
                    _issueCountControlVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private ErrorViewModel _errorViewModel;
        public ErrorViewModel ErrorViewModel
        {
            get => _errorViewModel;
            set
            {
                if (value != _errorViewModel)
                {
                    _errorViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        // Reset all
        public void Clear()
        {
            // Stop the active threads
            Cnt_Thread++;
            ClearHighLights();
            ClearActiveSolutions();
            ClearSolutions();
            ClearErrorMessage();
            UpdateStatus();
            Messenger.SendMessage(new ResetSnapshotMessage(), this);
        }

        // Unhighlight all the solutions
        public void ClearHighLights()
        {
            try
            {
                if (ActiveSolutions == null || ActiveSolutions.Count == 0)
                    return;

                bool isSaved = AssociatedDocument.Saved;
                foreach (var solution in ActiveSolutions)
                {
                    solution.ClearHighLight();
                }
                AssociatedDocument.Saved = isSaved;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        // Highlights all the active solutions
        public void ShowHighLights()
        {
            try
            {
                if (ActiveSolutions == null)
                    return;

                bool isSaved = AssociatedDocument.Saved;
                foreach (var solution in ActiveSolutions)
                {
                    solution.HighLight();
                }
                AssociatedDocument.Saved = isSaved;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        // Get the solution of the selection
        public void HandleMistakenWordSelection(Selection selection)
        {
            try
            {
                if (ActiveSolutions == null || ActiveSolutions.Count == 0)
                {
                    SelectedSolution = null;
                    return;
                }

                var solution = ActiveSolutions.FirstOrDefault(gc => selection.InRange(gc.Range) && gc.Range.IsValid() && gc.Highlighted);
                if (solution == null)
                {
                    SelectedSolution = null;
                    return;
                }

                SelectedSolution = solution;
                Messenger.SendMessage(new ShowSolutionsMessage(), this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public bool AllowMode(SolutionMode mode)
        {
            if (Solutions == null)
                return false;

            return Solutions.Any(s => s.Mode == mode);
        }

        public void UpdateLocalizationResources()
        {
            NoSuggestionText = Globals.GetString("TaskPane.NoSuggestion");
            ApplySolutionButtonToolTip = Globals.GetString("TaskPane.ApplySolutionButtonToolTip");
            IgnoreSolutionToolTip = Globals.GetString("TaskPane.IgnoreCorrection");
            
            NotifyPropertyChanged(nameof(ErrorViewModel));
            NotifyPropertyChanged(nameof(Status));
        }

        // Recheck all the text
        public void ReCheckAll()
        {
            // Stop the active threads
            Cnt_Thread++;
            // Clear all highlights of the active solutions.
            ClearHighLights();
            // Clear active solutions
            ClearActiveSolutions();
            // Clear all solutions
            ClearSolutions();
            SetCheckingStatus();
            ClearErrorMessage();
            Messenger.SendMessage(new ResetSnapshotMessage(), this);
        }

        // Remove all the active solutions
        private void ClearActiveSolutions()
        {
            if (ActiveSolutions != null)
                ActiveSolutions.Clear();

            SelectedSolution = null;
        }

        // Set the active solutions
        private void SetActiveSolutions()
        {
            var solutions = Solutions.GetSolutions(ActiveMode);
            ActiveSolutions = new ObservableCollection<GrammarSolutionModel>(solutions);
        }
        
        // Show solutions of the new mode
        private void DisplayActiveSolutions()
        {
            bool isSaved = AssociatedDocument.Saved;

            if (ActiveSolutions == null) return;

            foreach(var solution in ActiveSolutions)
            {
                if (solution.Highlighted == false)
                {
                    if (solution.Mode == _activeMode)
                    {
                        solution.HighLight();
                        solution.OnCorrected += OnApplySolution;
                    }
                }
            }
           
            AssociatedDocument.Saved = isSaved;
        }

        // Add solutions to Solutions
        private void AddSolutions(GrammarSolutions solutions)
        {
            if (Solutions == null)
                Solutions = new GrammarSolutions();
            
            Solutions.Add(solutions);
        }
        
        // Update paragraph index of solutions.
        private void UpdateParagraphIndex(GrammarSolutions newSolutions, List<ParagraphInfo> diffs, int indPSupp, int diffNbPar)
        {
            if(diffs.Count == 0 && indPSupp == -1) return;

            Paragraphs paragraphs = AssociatedDocument.Paragraphs;

            // 1. Update new solutions

            //Because we only send the changed paragraphs to server to re-check, let's say <paragraph 2> and <paragraph 5>
            //so we need to update the ParagraphIndex when we have the returned result

            foreach (var solution in newSolutions)
            {
                solution.ParagraphIndex = diffs[solution.ParagraphIndex - 1].Index;
            }

            // If no difference of paragraphs, don't do nothing.
            if (diffNbPar == 0 && indPSupp == -1) return;

            // 2. Update next solutions after new solutions
            if (Solutions != null)
            {
                // 1. Get the first next paragraphs after new solutions.
                int indexPNext = -1;

                // Suppression case
                if (indPSupp != -1) indexPNext = indPSupp;
                // Normal case
                else indexPNext = diffs[diffs.Count - 1].Index + 1;
                
                if (paragraphs.Count >= indexPNext)
                {
                    // Search the first solution in the next paragraphs.
                    // Don't ask much CPU because a solution of subjectivity is very probable in the next paragraph.
                    GrammarSolutionModel firstNextSolution = null;
                    var diffP = -1;
                    var indPFirstNextSolution = -1;

                    for (var i = indexPNext; i <= paragraphs.Count; i++)
                    {
                        Paragraph paragraph = paragraphs[i];
                        Range rangeP = paragraph.Range;

                        var solutions = Solutions.Where(gc => (rangeP.Start == gc.RangeP.Start)).ToList();
                        if (solutions != null && solutions.Count > 0)
                        {
                            firstNextSolution = solutions[0];
                            diffP = i - firstNextSolution.ParagraphIndex;
                            indPFirstNextSolution = firstNextSolution.ParagraphIndex;

                            break;
                        }
                    }

                    // Then assign paragraphs to the next solutions after firstNextSolution.
                    if (firstNextSolution != null)
                    {
                        // Solution.RangeP.Startr generating much CPU. Optimize with creating a map of indexP, index page 
                        var mapIndexP = new Dictionary<int, int>();

                        foreach (var solution in Solutions)
                        {
                            // Increment paragraph solution with ParagraphIndex >= firstNextSolution.ParagraphIndex
                            if (solution.ParagraphIndex >= indPFirstNextSolution)
                            {
                                solution.ParagraphIndex = solution.ParagraphIndex + diffP;

                                int paragraphIndex = solution.ParagraphIndex;
                                if (!mapIndexP.ContainsKey(paragraphIndex))
                                {
                                    int indexPageP = (int)solution.RangeP.Start;
                                    mapIndexP.Add(paragraphIndex, indexPageP);
                                }
                            }
                        }

                        // Assign Index_StartRange of solutions
                        foreach (var solution in Solutions)
                        {
                            int paragraphIndex = solution.ParagraphIndex;
                            if(mapIndexP.ContainsKey(paragraphIndex)) solution.Index_StartRange = mapIndexP[paragraphIndex];
                        }
                    }
                }
            }
        }

        // Recreate the range for range = null which are not in modified paragraphs
        // Special case where we replace all the text with another. Select all the paste.
        private void RecreateRangeP(List<ParagraphInfo> diffs, int indPSupp)
        {
            if (Solutions == null) return;
            
            if (diffs.Count > 0 && indPSupp == -1)
            {
                // Part before the change.
                int maxP = diffs[0].Index - 1;

                // Part after the change.
                //int minP = diffs[diffs.Count - 1].Index + 1;

                var invalidSolutions = Solutions.Where(gc => gc.Range == null || gc.Range.Text.IsNull()).ToList();
                foreach (var solution in invalidSolutions)
                {
                    if ((solution.ParagraphIndex <= maxP && maxP >= 0)/* ||     // Part before the change.
                        (solution.ParagraphIndex >= minP)*/)   // Part after the change. Not for the moment.
                    {
                        // Recreate the range.
                        Tuple<Range, int> t = AssociatedDocument.GetRangeInParagraph(solution.ParagraphIndex, solution.Start, solution.End);
                        Range range = t.Item1;
                        if (range != null)
                        {
                            solution.Range = range;
                            solution.OriginalWord = range.Text;
                            solution.RangeP = AssociatedDocument.Paragraphs[solution.ParagraphIndex].Range;
                            solution.Index_StartRange = t.Item2;
                            solution.Highlighted = false;
                        }
                    }
                }
            }
        }
        
        // Remove old solutions in paragraphs
        private void RemoveSolutionsInParagraphs(List<ParagraphInfo> diffs, int diffNbPar)
        {
            if (Solutions == null)
                return;

            HashSet<int> paragraphChangedIndices = new HashSet<int>();
            foreach (ParagraphInfo paragraphInfo in diffs) paragraphChangedIndices.Add(paragraphInfo.Index);

            // 1. Remove solutions which are in the modified paragraph.
            var solutionsToRemove = Solutions.Where(gc => paragraphChangedIndices.Contains(gc.ParagraphIndex)).ToList();
            foreach (var solution in solutionsToRemove)
            {
                Solutions.Remove(solution);

                if (ActiveSolutions != null)
                    ActiveSolutions.Remove(solution);
            }
            
            // 2. Remove invalid solutions (invalid Range due to user manually delete the word or pharse)
            var invalidSolutions = Solutions.Where(gc => gc.Range == null || gc.Range.Text.IsNull()).ToList();
            foreach (var solution in invalidSolutions)
            {
                Solutions.Remove(solution);

                if (ActiveSolutions != null)
                    ActiveSolutions.Remove(solution);
            }

            // 3. UnhighLigth all same background solutions

            var cnt = 0;
            Debug.WriteLine("UnhighLigth modified paragraphs");

            bool isSaved = AssociatedDocument.Saved;

            foreach (var i in paragraphChangedIndices)
            {
                // If we add new paragraph, don't remove the highlight. Remove highlight only for the first and the last paragraph. It asks veyr much ressources.
                if(diffNbPar > 0)
                {
                    cnt++;
                    if (!(cnt == 1 || cnt == paragraphChangedIndices.Count)) continue;
                }

                Paragraph paragraph = AssociatedDocument.Paragraphs[i];

                // Reach characters instead of words because a word can be partially highlighted. Fixed bug.
                var characters = paragraph.Range.Characters;
                foreach(var character in characters)
                {
                    var rangeC = (Range)character;

                    WdColor wdColorShading = rangeC.Font.Shading.BackgroundPatternColor;
                    if ((wdColorShading == GrammarSolutionModel.CorrectionHighlightColor/* && _activeMode == SolutionMode.Correction*/) ||
                        (wdColorShading == GrammarSolutionModel.StyleHighlightColor/* && _activeMode != SolutionMode.Correction*/))
                    {
                        rangeC.Font.Shading.BackgroundPatternColor = WdColor.wdColorAutomatic;
                    }
                }
            }

            AssociatedDocument.Saved = isSaved;
        }

        // Remove all solutions
        private void ClearSolutions()
        {
            if (Solutions != null)
                Solutions.Clear();
        }

        // Do it after replacing word
        private void OnApplySolution(object sender, EventArgs e)
        {
            var solution = (GrammarSolutionModel)sender;
            ActiveSolutions.Remove(solution);
            Solutions.Remove(solution);
            SelectedSolution = null;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (ActiveSolutions == null)
            {
                Status = null;
                NoIssueImageVisiblity = Visibility.Collapsed;
                IssueCountControlVisibility = Visibility.Collapsed;
                LoadingImageVisibility = Visibility.Collapsed;
                return;
            }

            if (ActiveSolutions.Count == 0)
            {
                Status = "TaskPane.NoIssuesFound";
                NoIssueImageVisiblity = Visibility.Visible;
                IssueCountControlVisibility = Visibility.Collapsed;
                LoadingImageVisibility = Visibility.Collapsed;
            }
            else
            {
                Status = "TaskPane.IssuesFound";
                IssueCountControlVisibility = Visibility.Visible;
                NoIssueImageVisiblity = Visibility.Collapsed;
                LoadingImageVisibility = Visibility.Collapsed;
            }
        }

        private void SetCheckingStatus()
        {
            Status = "TaskPane.Checking";
            LoadingImageVisibility = Visibility.Visible;
            IssueCountControlVisibility = Visibility.Collapsed;
            NoIssueImageVisiblity = Visibility.Collapsed;
        }

        // Set the range of the solution 
        private void SetRanges(GrammarSolutions solutionModel)
        {
            if (solutionModel == null)
                return;

            try
            {
                var mapIndexP = new Dictionary<int, int>();

                foreach (var solution in solutionModel)
                {
                    int paragraphIndex = solution.ParagraphIndex;

                    Tuple<Range, int> t = AssociatedDocument.GetRangeInParagraph(paragraphIndex, solution.Start, solution.End);
                    Range range = t.Item1;
                    if (range != null)
                    {
                        solution.Range = range;
                        solution.OriginalWord = range.Text;
                        solution.RangeP = AssociatedDocument.Paragraphs[paragraphIndex].Range;
                        solution.Index_StartRange = t.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void UpdateRibbon()
        {
            Globals.ThisAddIn.Ribbon.Invalidate();
        }


        private void InitializeCommands()
        {
            IgnoreSolutionCommand = new RelayCommand(IgnoreSelectedSolution);
        }

        private void ClearErrorMessage()
        {
            ErrorViewModel = null;
        }
        #endregion

        #region Commands
        public ICommand IgnoreSolutionCommand { get; set; }
        // Cancel a selected solution
        private void IgnoreSelectedSolution(object param)
        {
            if (SelectedSolution == null)
                return;

            SelectedSolution.ClearHighLight();
            ActiveSolutions.Remove(SelectedSolution);
            Solutions.Remove(SelectedSolution);
            SelectedSolution = null;
            UpdateStatus();
		}
		
        #endregion

        #region Localization

        private string _noSuggestionText;
        public string NoSuggestionText
        {
            get => _noSuggestionText;
            set
            {
                if (value != _noSuggestionText)
                {
                    _noSuggestionText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _applySolutionButtonToolTip;
        public string ApplySolutionButtonToolTip
        {
            get => _applySolutionButtonToolTip;
            set
            {
                if (value != _applySolutionButtonToolTip)
                {
                    _applySolutionButtonToolTip = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _ignoreSolutionToolTip;
        public string IgnoreSolutionToolTip
        {
            get => _ignoreSolutionToolTip;
            set
            {
                if (value != _ignoreSolutionToolTip)
                {
                    _ignoreSolutionToolTip = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Message Handler
        // Handle the message of starting
        public void HandleMessage(CheckingStartsMessage message)
        {
            SetCheckingStatus();
            ClearErrorMessage();
        }

        // Handle the message of completed checking
        public void HandleMessage(CheckingCompletedMessage message)
        {
            var newSolutions = message.Solutions;
            if (ActiveSolutions == null)
                ActiveSolutions = new ObservableCollection<GrammarSolutionModel>();

            if (newSolutions.LimiteNbChar != -1)
            {
                ErrorViewModel = new ExceedMaxCharacterErrorViewModel() { WindowHost = this.WindowHost };
            }
            else if (newSolutions.IsTrial)
            {
                ErrorViewModel = new ExceedTrialErrorViewModel() { WindowHost = this.WindowHost };
            }
            else
            {
                ErrorViewModel = null;
            }

        	var diffs = message.ParagraphIndices;
            var indPSupp = message.IndPSupp;
            var diffNbPar = message.DiffNbPar;

            Debug.WriteLine("F1");
            UpdateParagraphIndex(newSolutions, diffs, indPSupp, diffNbPar);     // Update paragraphs index
            Debug.WriteLine("F2");
            RecreateRangeP(diffs, indPSupp);    // Special case of replacing all. Recreate the range for range = null which are not in modified paragraphs.
            Debug.WriteLine("F3");
            RemoveSolutionsInParagraphs(diffs, diffNbPar);  // Remove solution belonging to paragraphs.
            Debug.WriteLine("F4");
            SetRanges(newSolutions);    // Set ranges to new solutions.
            Debug.WriteLine("F5");
            AddSolutions(newSolutions);     // Add new solutions
            Debug.WriteLine("F6");
            ClearActiveSolutions();     // Clear active solutions
            Debug.WriteLine("F7");
            SetActiveSolutions();   // Create active solutions
            Debug.WriteLine("F8");
            DisplayActiveSolutions();  // Display active solutions
            Debug.WriteLine("F9");

            UpdateStatus();
            UpdateRibbon();
        }

        // Handle the message of limit characters
        public void HandleMessage(LimCharExceededMessage message)
        {
            ErrorViewModel = new ExceedMaxCharacterErrorViewModel() { WindowHost = this.WindowHost };
        }
        
        // Handle the message of error
        public void HandleMessage(ShowErrorMessage message)
        {
            ErrorViewModel = message.ErrorViewModel;
        }
        #endregion
    }
}
