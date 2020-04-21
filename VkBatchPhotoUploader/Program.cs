using System;
using System.Threading.Tasks;

namespace VkBatchPhotoUploader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var vkAppSettings = new VkAppSettings();
            var consoleDialogManager = new ConsoleDialogManager();

            var vkAuthenticator = new VkAuthenticator(vkAppSettings);

            string code = consoleDialogManager.AskCode();
            var authorizedApi = vkAuthenticator.AuthorizeAsync(code).Result;

            var vkPhotoUploader = new VkPhotoUploader(authorizedApi, 
                consoleDialogManager);

            Console.ReadKey();
        }
    }
}
