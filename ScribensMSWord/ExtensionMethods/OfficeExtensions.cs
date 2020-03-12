using Microsoft.Office.Interop.Word;
using System;
using System.Threading;
using PluginScribens_Word.Utils;

namespace PluginScribens_Word.ExtensionMethods
{
    public static class OfficeExtensions
    {
        public delegate void AfterSaveDelegate(Document doc);

        public static void SubscribeAfterSave(this Application application, AfterSaveDelegate callback)
        {
            application.DocumentBeforeSave += delegate (Document document, ref bool saveAsUI, ref bool cancel)
            {
                OnDocumentBeforeSave(document, saveAsUI, callback);
            };
        }

        public static void UnsubscribeAfterSave(this Application application)
        {
            //TODO
        }

        private static void OnDocumentBeforeSave(Document document, bool uiSave, AfterSaveDelegate callback)
        {
            new Thread(() =>
            {
                try
                {
                    // we have a UI save, so we need to get stuck
                    // here until the user gets rid of the SaveAs dialog
                    if (uiSave)
                    {
                        while (document.IsBusy())
                            Thread.Sleep(1);
                    }

                    // check to see if still saving in the background
                    // we will hang here until this changes.
                    while (document.Application.BackgroundSavingStatus > 0)
                        Thread.Sleep(1);

                    callback.Invoke(document);
                }
                catch (Exception ex)
                {
                    Logger.Info(ex.Message);
                }
            }).Start();
        }

        private static bool IsBusy(this Document document)
        {
            try
            {
                // if we try to access the application property while
                // Word has a dialog open, we will fail
                object o = document.Application;
                return false; // not busy
            }
            catch
            {
                // so, Word is busy and we return true
                return true;
            }
        }
    }
}
