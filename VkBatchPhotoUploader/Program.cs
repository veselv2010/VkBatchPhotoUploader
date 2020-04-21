using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VkNet;
using VkNet.Model;
using Newtonsoft.Json.Linq;

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
