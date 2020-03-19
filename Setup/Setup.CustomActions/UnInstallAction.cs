
using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;

namespace Setup.CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult UnInstallAction(Session session)
        {
            var installFolder = session["INSTALLFOLDER"];
            DeleteInstallFiles(installFolder);
            DeleteRegistryEntries();

            return ActionResult.Success;
        }

        private static void DeleteInstallFiles(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private static void DeleteRegistryEntries()
        {
            try
            {
                string wordAddinKey = @"Software\Microsoft\Office\Word\Addins";
                string excelAddinKey = @"Software\Microsoft\Office\Excel\Addins";
                string powerPointAddinKey = @"Software\Microsoft\Office\PowerPoint\Addins";
                string outlookAddinKey = @"Software\Microsoft\Office\Outlook\Addins";

                using (var regKey = Registry.CurrentUser.OpenSubKey(wordAddinKey, true))
                {
                    if (regKey != null)
                        regKey.DeleteSubKey("Scribens");
                }
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }
        }

        private static void Log(string content)
        {
            //File.WriteAllText(@"c:\temp\log.txt", content);
        }
    }
}
