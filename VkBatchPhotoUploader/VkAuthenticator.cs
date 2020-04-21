using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using VkNet;
using VkNet.Model;
using Newtonsoft.Json.Linq;

namespace VkBatchPhotoUploader
{
    class VkAuthenticator
    {
        public VkApi api { get; }
        public VkAuthenticator(ConsoleDialogManager consoleDialog)
        {
            string url = "https://oauth.vk.com/authorize?client_id=7096347&display=page&scope=photos&redirect_uri=http://blank.org/&response_type=code&v=5.103";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });

                consoleDialog.DisplayCodeRequest();
                string code = Console.ReadLine();

                var tokenParamsDict = new Dictionary<string, string>();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.vk.com/access_token");

                tokenParamsDict.Add("client_id", "7096347");
                tokenParamsDict.Add("client_secret", "dfem1KnHOVrDN21VHckc");
                tokenParamsDict.Add("redirect_uri", "http://blank.org/");
                tokenParamsDict.Add("code", code);

                request.Content = new FormUrlEncodedContent(tokenParamsDict);

                using (var client = new HttpClient())
                using (this.api = new VkApi())
                {
                    var resp = client.SendAsync(request).Result;
                    var respContent = resp.Content.ReadAsStringAsync().Result;

                    var accessToken = JObject.Parse(respContent)
                        .GetValue("access_token").ToString();

                    api.Authorize(new ApiAuthParams
                    {
                        AccessToken = accessToken
                    });
                }              
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
