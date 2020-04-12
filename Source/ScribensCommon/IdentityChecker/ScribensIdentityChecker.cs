using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using PluginScribens.Checker;
using PluginScribens.Common.Enums;
using PluginScribens.Common.ExtensionMethods;

namespace PluginScribens.Common.IdentityChecker
{
    public class ScribensIdentityChecker : IIdentityChecker
    {
        public async Task<Identity> CheckIdentityAsync(string username, string password, string language)
        {
            var param = HttpUtility.ParseQueryString(string.Empty);
            param.Add("FunctionName", "EstAbonne");
            param.Add("Id", username);
            param.Add("Password", password);
            var postData = Encoding.ASCII.GetBytes(param.ToString());

            string host = ScribensServers.GetHost(language);
            var request = (HttpWebRequest)WebRequest.Create($"{host}/{ScribensServers.GetDefaultServerName()}/Identification_Servlet");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            
            using (var stream = request.GetRequestStream())
            {
                await stream.WriteAsync(postData, 0, postData.Length);
                await stream.FlushAsync();
            }

            try
            {
                using (var response = await request.GetResponseAsync())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string responseData = await streamReader.ReadToEndAsync();
                    var dataArray = JsonConvert.DeserializeObject<string[]>(responseData);

                    var identity = new Identity();
                    identity.Status = dataArray[0].ToEnum<IdentityStatus>();
                    identity.Username = dataArray[2];
                    identity.Password = dataArray[8];
                    identity.Email = dataArray[7];
                    identity.SubscriptionType = dataArray[9].ToEnum<SubscriptionType>();
                    identity.ExpiredDate = dataArray[10].ToDateTime();
                    identity.LastSubscriptionExpiredDate = dataArray[11].ToDateTime();
                    identity.Options = dataArray[16];
                    
                    return identity;
                }
            }
            catch (Exception ex)
            {
                request.Abort();
                Logger.Error(ex);
                throw;
            }
        }
    }
}
