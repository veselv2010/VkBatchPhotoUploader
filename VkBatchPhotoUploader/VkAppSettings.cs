using System;

namespace VkBatchPhotoUploader
{
    public class VkAppSettings
    {
        public string clientId { get; }
        public string clientSecret { get; }
        public string redirectUri { get; }
        public VkAppSettings(string clientId, string clientSecret, string redirectUri)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.redirectUri = redirectUri;
        }
    }
}
