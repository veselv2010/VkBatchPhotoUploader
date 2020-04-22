using System;

namespace VkBatchPhotoUploader
{
    public class VkAppSettings
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string RedirectUri { get; }
        public VkAppSettings(string clientId, string clientSecret, string redirectUri)
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.RedirectUri = redirectUri;
        }
    }
}
