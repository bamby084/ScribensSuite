using Newtonsoft.Json;
using System;
using System.IO;

namespace ScribensMSWord.Utils
{
    public class LoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastLoginDate { get; set; }

        public static string GetFilePath()
        {
            string appData = Settings.GetAppDataFolder();
            return Path.Combine(appData, "identity.json");
        }

        public static LoginInfo Load()
        {
            try
            {
                string filePath = GetFilePath();
                if (!File.Exists(filePath))
                    return null;

                using (var textReader = new StreamReader(filePath))
                {
                    var loginInfo = JsonConvert.DeserializeObject<LoginInfo>(textReader.ReadToEnd());
                    return loginInfo;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public static void Save(string username, string password)
        {
            var loginInfo = new LoginInfo()
            {
                Username = username,
                Password = password
            };

            loginInfo.Save();
        }

        public static void Delete()
        {
            try
            {
                string filePath = GetFilePath();
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Save()
        {
            try
            {
                string appDataPath = Settings.GetAppDataFolder();
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                this.LastLoginDate = DateTime.UtcNow;
                string filePath = GetFilePath();
                string data = JsonConvert.SerializeObject(this);

                File.WriteAllText(filePath, data);
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
