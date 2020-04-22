using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using VkNet;
using VkNet.Model;
using VkNet.Abstractions;
using Newtonsoft.Json.Linq;

namespace VkBatchPhotoUploader
{
    class VkAuthenticator
    {
        private readonly IDialogManager dialogManager;
        private readonly HttpClient client;
        private readonly VkAppSettings settings;

        public VkAuthenticator(VkAppSettings settings, IDialogManager dialogManager)
        {
            this.client = new HttpClient();
            this.settings = settings;
            this.dialogManager = dialogManager;
        }

        public void OpenCodePage()
        {
            string url = $"https://oauth.vk.com/authorize?client_id={settings.ClientId}&" +
                $"display=page&scope=photos&redirect_uri={settings.RedirectUri}&response_type=code&v=5.103";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });

                dialogManager.DisplayMessage("code = ");
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var tokenParamsDict = CreateDictionaryFrom(code);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.vk.com/access_token");

            request.Content = new FormUrlEncodedContent(tokenParamsDict);

            var resp = await client.SendAsync(request);
            var respContent = await resp.Content.ReadAsStringAsync();

            return JObject.Parse(respContent)
                .GetValue("access_token").ToString();
        }

        public async Task<IVkApi> GetAuthorizedApiAsync(string accessToken)
        {
            var api = new VkApi();
            await api.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = accessToken
            });

            return api;
        }

        private Dictionary<string, string> CreateDictionaryFrom(string code)
        {
            return new Dictionary<string, string>
            {
                { "client_id", this.settings.ClientId },
                { "client_secret", this.settings.ClientSecret },
                { "redirect_uri", this.settings.RedirectUri },
                { "code", code }
            };
        }
    }
}
