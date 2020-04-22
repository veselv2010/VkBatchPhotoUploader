using System;
using System.Threading.Tasks;

namespace VkBatchPhotoUploader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var vkAppSettings = new VkAppSettings("7096347",
                "dfem1KnHOVrDN21VHckc", "http://blank.org/");

            var dialogManager = new DialogManager();

            var vkAuthenticator = new VkAuthenticator(vkAppSettings);
            vkAuthenticator.Display += dialogManager.DisplayMessage;

            vkAuthenticator.OpenCodePage();
            string code = dialogManager.Ask();
            string accessToken = vkAuthenticator.GetAccessTokenAsync(code).Result;

            var api = vkAuthenticator.GetAuthorizedApiAsync(accessToken).Result;

            var vkPhotoUploader = new VkPhotoUploader(api);

            vkPhotoUploader.Display += dialogManager.DisplayMessage;
            vkPhotoUploader.DisplayAlbumsRequest += dialogManager.DisplayMessage;
            vkPhotoUploader.DisplayException += dialogManager.DisplayMessage;
            vkPhotoUploader.Ask += dialogManager.Ask;

            string[] photos = vkPhotoUploader.GetFolderFiles();
            long albumId = vkPhotoUploader.AlbumSelector();

            vkPhotoUploader.UploadPhotos(albumId, photos);
            
            Console.ReadKey();
        }
    }
}
