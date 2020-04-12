using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using PluginScribens.Common;
using PluginScribens.Common.Enums;
using PluginScribens.Common.ExtensionMethods;
using PluginScribens_Word.Properties;

namespace PluginScribens_Word
{
    [ComVisible(true)]
    public class AddinRibbon : Office.IRibbonExtensibility
    {
        private string _selectedLanguage;
        private Office.IRibbonUI _ribbon;

        #region ctors
        public AddinRibbon()
        {
            _selectedLanguage = Plugin.Settings.Language.Abbreviation;
        }
        #endregion

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("PluginScribens_Word.AddinRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226
        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this._ribbon = ribbonUI;
        }

        public void BackStage_Show(object context)
        {
            Globals.BackStageVisible = true;
        }

        public void BackStage_Hide(object context)
        {
            Globals.BackStageVisible = false;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Custom Code
        public Bitmap GetImage(Office.IRibbonControl control)
        {
            switch (control.Id)
            {
                case "SettingsButton":
                    return new Bitmap(Resources.settings);
                case "ShowTaskPaneButton":
                    return new Bitmap(Resources.scribens);
                case "CorrectionModeButton":
                    return new Bitmap(Resources.verification2);
                case "RedundancyModeButton":
                    return new Bitmap(Resources.redundancy);
                case "LongSentenceModeButton":
                    return new Bitmap(Resources.phrase);
                case "ReformulationModeButton":
                    return new Bitmap(Resources.reformulation); 
                case "VocabularyEnhancementModeButton":
                    return new Bitmap(Resources.vocabulary_enhancement);
                case "SubjectivityPositiveModeButton":
                    return new Bitmap(Resources.subjective_positive);
                case "SubjectivityNegativeModeButton":
                    return new Bitmap(Resources.subjective_negative);
                case "WordReductionModeButton":
                    return new Bitmap(Resources.word_reduction);
                case "ShowUserAccountButton":
                    return new Bitmap(Resources.account);
            }

            return null;
        }

        public Bitmap GetImageByName(string imageName)
        {
            switch (imageName)
            {
                case "england-flag.png":
                    return new Bitmap(Resources.england_flag, new Size(20, 20));
                case "french-flag.png":
                    return new Bitmap(Resources.french_flag, new Size(20, 20));
                default:
                    return null;
            }
        }

        public string GetLabel(Office.IRibbonControl control)
        {
            if (control.Id == "ShowTaskPaneButton")
            {
                var window = control.GetWindow();
                if (window == null)
                    return Globals.GetString("Ribbon.ShowTaskPaneButton");

                var taskPane = window.GetTaskPane();
                if (taskPane == null || !taskPane.Visible)
                    return Globals.GetString("Ribbon.ShowTaskPaneButton");

                return Globals.GetString("Ribbon.CloseTaskPane");
            }
            else
            {
                return Globals.GetString($"Ribbon.{control.Id}");
            }
        }

        public string InitLanguageDropDown(Office.IRibbonControl control)
        {
            return _selectedLanguage;
        }

        public void OnLanguageChanged(Office.IRibbonControl control, string selectedId, string selectedIndex)
        {
            _selectedLanguage = selectedId;
            Globals.Settings.Language = Utils.Language.Find(_selectedLanguage);
            Globals.Settings.Save();
            Globals.ThisAddIn.RefreshRibbon();
            Globals.Windows.UpdateTaskPanes();
            ReCheck(control);
        }

        public void ShowSettingsPage(Office.IRibbonControl control)
        {
            var settings = new SettingsPageHost();
            settings.Width = 600;
            settings.Height = 375;
            settings.ShowDialog();
        }

        public void ShowUserInfo(Office.IRibbonControl control)
        {
            if (control.Context == null)
                return;

            IWindow window = control.GetWindow();
            if (window == null)
            {
                window = Globals.Windows.AddNewWindow(((Window)control.Context).Hwnd);
            }

            window.ShowUserInfo();
        }

        public void ShowSolutions(Office.IRibbonControl control)
        {
            if (control.Context == null)
                return;

            IWindow window = control.GetWindow();
            if (window == null)
            {
                window = Globals.Windows.AddNewWindow(((Window)control.Context).Hwnd);
            }

            window.ShowSolutions();
        }

        public void OnSolutionButtonSwitch(Office.IRibbonControl control, bool isPressed)
        {
            IWindow window = control.GetWindow();
            if (window == null)
                return;

            window.ActiveMode = control.Tag.ToEnum<SolutionMode>();
            _ribbon.Invalidate();
        }

        public bool SolutionButtonGetPressed(Office.IRibbonControl control)
        {
            IWindow window = control.GetWindow();
            if (window == null)
                return false;

            return control.Tag == window.ActiveMode.ToString();
        }

        public bool SolutionButtonGetEnabled(Office.IRibbonControl control)
        {
            IWindow window = control.GetWindow();
            if (window == null)
                return false;

            var mode = control.Tag.ToEnum<SolutionMode>();
            return window.AllowMode(mode);
        }

        public void Invalidate()
        {
            _ribbon.Invalidate();
        }

        public void Invalidate(string control)
        {
            _ribbon.InvalidateControl(control);
        }

        public void ReCheck(Office.IRibbonControl control)
        {
            if (control.Context == null)
                return;

            IWindow window = control.GetWindow();
            if (window == null)
            { 
                window = Globals.Windows.AddNewWindow(((Window)control.Context).Hwnd);
            }

            window.ReCheck();
        }
        #endregion
    }
}
