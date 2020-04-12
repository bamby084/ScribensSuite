using System;
using System.Collections.Generic;
using Timer = System.Timers.Timer;
using System.Threading;
using System.Diagnostics;
using Microsoft.Office.Interop.Word;
using PluginScribens.Common.ExtensionMethods;


namespace PluginScribens.Common.GrammarChecker
{
    public class CheckCompletedEventArgs : EventArgs
    {
        // Solutions from the checking request
        public GrammarSolutions Solutions { get; set; }
        // Index of modified paragraphs
        public List<ParagraphInfo> ParagraphIndices { get; set; }
        // Index of indice paragraph of removing. Only when removing paragraph change.
        public int IndPSupp = -1;
        // Difference of number of paragraph counter in the change.
        public int DiffNbPar = -1;

        // Event of check completed
        public CheckCompletedEventArgs()
        {

        }

        // Event of check completed
        public CheckCompletedEventArgs(GrammarSolutions solutions, List<ParagraphInfo> paragraphIndices, int indPSupp, int diffNbPar)
        {
            Solutions = solutions;
            ParagraphIndices = paragraphIndices;
            IndPSupp = indPSupp;
            DiffNbPar = diffNbPar;
        }
    }

    // Event of start checking
    public class StartCheckingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }

    // Event of character limitation
    public class LimCharExceededEventArgs : EventArgs
    {
    }
    
    public delegate void CheckCompletedEventHandler(object sender, CheckCompletedEventArgs e);
    public delegate void StartCheckingEventHandler(object sender, StartCheckingEventArgs e);
    public delegate void BeforeCheckingEventHandler(object sender, ref bool cancel);
    public delegate void LimCharExceededEventHandler(object sender, LimCharExceededEventArgs e);
    
    public class BackgroundChecker
    {
        public event CheckCompletedEventHandler OnCheckCompleted;
        public event StartCheckingEventHandler OnStartChecking;
        public event BeforeCheckingEventHandler OnBeforeChecking;
        public event LimCharExceededEventHandler OnLimCharExceeded;
        
        // Timer of changing text
        private Timer _timer;
        // Previous document text
        private string _lastDocumentText = "";
        // Lock for modified text
        public Boolean Lock = false;
        // Locnk of scrollbar
        public Boolean LockScrollBar = false;

        public bool IsEnabled { get; set; }

        public IGrammarChecker GrammarChecker { get; private set; }

        // Current document
        public Document Document { get; private set; }

        public BackgroundChecker(Document document, IGrammarChecker grammarChecker)
        {
            Document = document;
            GrammarChecker = grammarChecker;
        }

        // Star the timers
        public void Start()
        {
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Interval = Plugin.Settings.BackgroundCheckingInterval * 1000;
                _timer.Elapsed += OnTimerElapsed;
            }

            _timer.Start();
        }
        
        // Stop the timer
        public void Stop()
        {
            if (_timer != null)
                _timer.Stop();
        }

        // Rest the last document text
        public void ResetSnapshot()
        {
            _lastDocumentText = null;
        }

        // Function of modified text
        private async void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsEnabled || !Plugin.Settings.AllowBackgroundChecking)
                return;

            if(OnBeforeChecking != null)
            {
                bool isCancel = false;
                OnBeforeChecking(this, ref isCancel);

                if (isCancel)
                    return;
            }

            if (Lock == false)
            {
                await TaskPool.StartNew(CheckGrammar);
            }
        }

        // Function to detect modification on text
        private void CheckGrammar(CancellationToken token, Guid taskId)
        {
            if (Document == null)
                return;

            //cancel other running tasks
            TaskPool.CancelRunningTasks(taskId);

            // Check if the text has changed.
            try
            {
                if (!Document.Content.Text.Equals(_lastDocumentText))   // Sometimes, this function crashes. Ex : Il changes et arrives. Check then put a link on arrives.
                {
                    Lock = true;

                    var snapshotText = _lastDocumentText;
                    var saveCurrentText = Document.Content.Text;

                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine($"Task {taskId} cancelled due to requested");
                        return;
                    }

                    Tuple<List<ParagraphInfo>, int, int> pDiff = Document.GetDiffs(snapshotText);
                    List<ParagraphInfo> diffs = pDiff.Item1;
                    int indPSupp = pDiff.Item2;
                    int diffNbPar = pDiff.Item3;
                    var textToCheck = diffs.Join();

                    // Check the character limit length before sending to the server, because it save CPU.
                    if (textToCheck.Length > Plugin.Settings.LimitedCharacters)
                    {
                        OnLimCharExceeded?.Invoke(this, new LimCharExceededEventArgs());
                        Lock = false;
                        return;
                    }

                    // Clear the message on taskpane. Show the waiting .gif
                    if (OnStartChecking != null)
                    {
                        var startCheckingArgs = new StartCheckingEventArgs();
                        OnStartChecking(this, startCheckingArgs);

                        if (startCheckingArgs.Cancel)
                        {
                            Debug.WriteLine($"Task {taskId} cancelled by event handler");
                            return;
                        }
                    }
                    var solutions = GrammarChecker.CheckAsync(textToCheck, Plugin.Settings.Language.Abbreviation).Result;
                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine($"Task {taskId} cancelled due to requested");
                        return;
                    }

                    if (!Document.Content.Text.Equals(saveCurrentText))
                    {
                        Lock = false;
                        return;
                    }

                    //var prevTx = Document.Content.Text;

                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine($"Task {taskId} cancelled due to requested");
                        return;
                    }

                    OnCheckCompleted?.Invoke(this, new CheckCompletedEventArgs(solutions, diffs, indPSupp, diffNbPar));

                    //if(!prevTx.Equals(Document.Content.Text)) Debug.WriteLine($"XXXX");

                    _lastDocumentText = Document.Content.Text;

                    //Debug.WriteLine($"Task {taskId} has solutions");
                    //Debug.WriteLine("Last document text : " + _lastDocumentText);

                    Lock = false;
                }
            }
            catch (Exception e) { Debug.WriteLine(e.Message); }
        }
    }
}
