using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PluginScribens.Checker;

namespace PluginScribens.Common.SessionChecker
{
    public class ScribensSessionChecker : ISessionChecker
    {
        public async Task NotifyAsync(string userName, string language)
        {
            Debug.WriteLine("Notifying Server...");

            var param = HttpUtility.ParseQueryString(string.Empty);
            param.Add("FunctionName", "SignalSessionActive");
            param.Add("Id", userName);
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
                {
                    if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                    {
                        using (var streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            string responseData = await streamReader.ReadToEndAsync();
                            Logger.Error($"Session::NotifyServer failed: {responseData}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                request.Abort();
                Logger.Error(ex);
            }
        }
    }
}
