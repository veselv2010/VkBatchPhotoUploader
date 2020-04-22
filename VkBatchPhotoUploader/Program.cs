using System;

namespace VkBatchPhotoUploader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var vkAppSettings = new VkAppSettings("7096347",
                "dfem1KnHOVrDN21VHckc", "http://blank.org/");

            var dialogManager = new ConsoleDialogManager();

            var vkAuthenticator = new VkAuthenticator(vkAppSettings, dialogManager);

            vkAuthenticator.OpenCodePage();
            string code = dialogManager.Ask();
            string accessToken = vkAuthenticator.GetAccessTokenAsync(code).Result;

            var api = vkAuthenticator.GetAuthorizedApiAsync(accessToken).Result;

            var vkPhotoUploader = new VkPhotoUploader(api, dialogManager);

            string[] photos = vkPhotoUploader.GetFolderFiles();
            long albumId = vkPhotoUploader.AlbumSelector();

            vkPhotoUploader.UploadPhotos(albumId, photos);
            
            Console.ReadKey();
        }
    }
}
