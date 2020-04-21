using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using VkNet;
using VkNet.Model;
using Newtonsoft.Json.Linq;

namespace VkBatchPhotoUploader
{
    class VkAuthenticator
    {
        private HttpClient client;
        private VkApi api;
        private VkAppSettings settings;
        public VkAuthenticator(VkAppSettings settings)
        {
            this.api = new VkApi();
            this.client = new HttpClient();
            this.settings = settings;

            string url = $"https://oauth.vk.com/authorize?client_id={settings.client_id}&" +
                $"display=page&scope=photos&redirect_uri={settings.redirect_uri}&response_type=code&v=5.103";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        async public Task<VkApi> AuthorizeAsync(string code)
        {
            var tokenParamsDict = new Dictionary<string, string>
                {
                    { "client_id", this.settings.client_id },
                    { "client_secret", this.settings.client_secret },
                    { "redirect_uri", this.settings.redirect_uri },
                    { "code", code }
                };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.vk.com/access_token");

            request.Content = new FormUrlEncodedContent(tokenParamsDict);

            var resp = await client.SendAsync(request);
            var respContent = await resp.Content.ReadAsStringAsync();

            string accessToken = JObject.Parse(respContent)
                .GetValue("access_token").ToString();

            await api.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = accessToken
            });

            return api;
        }
    }
}
