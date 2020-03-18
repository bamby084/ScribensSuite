
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
            DeleteRegistry();

            return ActionResult.Success;
        }

        private static void DeleteInstallFiles(string path)
        {
            try
            {
                Log(path);
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }

        private static void DeleteRegistry()
        {
            try
            {
                string msWordAddinKey = @"Software\Microsoft\Office\Word\Addins";
                using (var regKey = Registry.CurrentUser.OpenSubKey(msWordAddinKey, true))
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
