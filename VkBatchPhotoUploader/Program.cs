using System;

namespace VkBatchPhotoUploader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var consoleDialogManager = new ConsoleDialogManager();

            var vkAuthenticator = new VkAuthenticator(consoleDialogManager);

            var vkPhotoUploader = new VkPhotoUploader(vkAuthenticator.api, 
                consoleDialogManager);

            Console.ReadKey();
        }
    }
}
