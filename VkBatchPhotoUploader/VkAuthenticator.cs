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

            string url = $"https://oauth.vk.com/authorize?client_id={settings.clientId}&" +
                $"display=page&scope=photos&redirect_uri={settings.redirectUri}&response_type=code&v=5.103";

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
        async public Task<string> GetAccessTokenAsync(string code)
        {
            var tokenParamsDict = new Dictionary<string, string>
                {
                    { "client_id", this.settings.clientId },
                    { "client_secret", this.settings.clientSecret },
                    { "redirect_uri", this.settings.redirectUri },
                    { "code", code }
                };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.vk.com/access_token");

            request.Content = new FormUrlEncodedContent(tokenParamsDict);

            var resp = await client.SendAsync(request);
            var respContent = await resp.Content.ReadAsStringAsync();

            return JObject.Parse(respContent)
                .GetValue("access_token").ToString();
        }
        async public Task<VkApi> GetAuthorizedApiAsync(string accessToken)
        {
            await api.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = accessToken
            });

            return api;
        }
    }
}
