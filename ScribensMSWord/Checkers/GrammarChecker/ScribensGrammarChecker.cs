using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System;
using System.Linq;
using ScribensMSWord.Utils;
using ScribensMSWord.ExtensionMethods;

namespace ScribensMSWord.Checkers.GrammarChecker.Scribens
{
    public class ScribensGrammarChecker : IGrammarChecker
    {
        // Launch the request of checking
        public async Task<GrammarSolutions> CheckAsync(string text, string language)
        {
            if(text.Length > Globals.Settings.LimitedCharacters)
            {
                return new GrammarSolutions() { LimiteNbChar = Globals.Settings.LimitedCharacters };
            }

            var param = HttpUtility.ParseQueryString(string.Empty);
            param.Add("FunctionName", "GetSolutionsByPos");
            param.Add("optionsCor", BuildOptionsCor());
            param.Add("optionsStyle", BuildOptionsStyle());
            param.Add("langId", language);
            param.Add("plugin", "MSWord");
            param.Add("texteHTML", $"{text}");

            //Debug.WriteLine($"Text send to the server {text}");

            if (Globals.CurrentIdentity != null)
            {
                param.Add("identifier", Globals.CurrentIdentity.Email);
                param.Add("password", Globals.CurrentIdentity.Password);
            }
            
            string host = ScribensServers.GetHost(language);
            var request = (HttpWebRequest)WebRequest.Create($"{host}/{ScribensServers.GetServerName()}/TextSolution_Servlet");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.Timeout = 3600000;   // Set the timout to 1 hour.

            try
            {
                var postData = Encoding.ASCII.GetBytes(param.ToQueryString());
                using (var stream = request.GetRequestStream())
                {
                    await stream.WriteAsync(postData, 0, postData.Length);
                    await stream.FlushAsync();
                }

                using (var response = await request.GetResponseAsync())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = await streamReader.ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<GrammarCheckingResult>(responseData);

                    return data.ToGrammarSolutions();
                }
            }
            catch (Exception ex)
            {
                request.Abort();
                Logger.Error(ex);
                return new GrammarSolutions();
            }
        }

        // Get the settings for cor
        private string BuildOptionsCor()
        {
            string baseOptions = "Genre_Je:0|Genre_Tu:0|Genre_Nous:0|Genre_Vous:0|Genre_On:0";
            string defaultOptions = $"{baseOptions}|RefOrth:0|ShowUPSol:1|UsBr:-1";

            string[] keys = new string[] { "RefOrth", "ShowUPSol", "UsBr" };

            if (Globals.CurrentIdentity == null || Globals.CurrentIdentity.Options.IsNull())
                return defaultOptions;

            try
            {
                var options = Globals.CurrentIdentity.Options.Split('|').Select(item => item.Split(':')).ToDictionary(s => s[0], s => s[1]);
                string result = baseOptions;

                foreach (string key in keys)
                {
                    if (options.ContainsKey(key))
                    {
                        result += $"|{key}:{options[key]}";
                    }
                }

                return result;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return defaultOptions;
            }
        }

        // Get the settings for style
        private string BuildOptionsStyle()
        {
            string defaultOptions = "RefOrth:0|ShowUPSol:1|UsBr:-1|RepMin:3|GapRep:3|AllWords:0|FamilyWords:0|MinPhLg:30|MinPhCt:5|Ttr:250|Tts:150";

            if (Globals.CurrentIdentity == null || Globals.CurrentIdentity.Options.IsNull())
                return defaultOptions;

            try
            {
                string[] keys = new string[] { "RepMin", "GapRep", "AllWords", "FamilyWords", "MinPhLg", "MinPhCt", "Ttr", "Tts" };
                var options = Globals.CurrentIdentity.Options.Split('|').Select(item => item.Split(':')).ToDictionary(s => s[0], s => s[1]);
                string result = string.Empty;

                foreach (string key in keys)
                {
                    if (options.ContainsKey(key))
                    {
                        result += $"{key}:{options[key]}|";
                    }
                }

                if (result.EndsWith("|"))
                    result = result.Substring(0, result.Length - 1);

                return result;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return defaultOptions;
            }
        }
    }
}
