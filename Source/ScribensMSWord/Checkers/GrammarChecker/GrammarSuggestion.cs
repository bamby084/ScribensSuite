using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PluginScribens_Word.WPF;

namespace PluginScribens_Word.Checkers.GrammarChecker
{
    public class ApplySuggestionEventHandlerArgs : EventArgs
    {
        public string Suggestion { get; set; }
        public ApplySuggestionEventHandlerArgs(string suggestion)
        {
            Suggestion = suggestion;
        }
    }
    public delegate void ApplySuggestionEventHandler(object sender, ApplySuggestionEventHandlerArgs e);

    public class GrammarSuggestion: INotifyPropertyChanged
    {
        #region Properties
        public event ApplySuggestionEventHandler OnApplySuggestion;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Original { get; set; }

        public string Suggestion { get; set; }
        #endregion

        #region ctors

        public GrammarSuggestion()
            : this(null)
        {
        }

        public GrammarSuggestion(string suggestion)
        {
            Suggestion = suggestion;
            ApplySuggestionCommand = new RelayCommand(ApplySuggestion);
        }
        #endregion

        #region Commands
        public ICommand ApplySuggestionCommand { get; set; }

        private void ApplySuggestion(object param)
        {
            OnApplySuggestion?.Invoke(this, new ApplySuggestionEventHandlerArgs(Suggestion));
        }
        #endregion

        #region INotifyPropertyChanged
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
