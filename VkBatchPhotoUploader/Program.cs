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

            var consoleDialogManager = new ConsoleDialogManager();

            var vkAuthenticator = new VkAuthenticator(vkAppSettings);

            string code = consoleDialogManager.AskCode();
            string accessToken = vkAuthenticator.GetAccessTokenAsync(code).Result;

            var api = vkAuthenticator.GetAuthorizedApiAsync(accessToken).Result;

            var vkPhotoUploader = new VkPhotoUploader(api, 
                consoleDialogManager);

            Console.ReadKey();
        }
    }
}
