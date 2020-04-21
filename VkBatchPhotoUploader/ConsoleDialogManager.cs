using System;
using System.Collections.Generic;
using VkNet.Utils;
using VkNet.Model;

namespace VkBatchPhotoUploader
{
    class ConsoleDialogManager
    {
        public string AskCode()
        {
            Console.Write("code = ");
            return Console.ReadLine();
        }

        public string AskAlbum()
        {
            Console.Write("# of desired album: ");
            return Console.ReadLine();
        }

        public void DisplayAlbumRequest(IList<PhotoAlbum> albums)
        {
            var rnd = new Random();
            for(int i = 0; i < albums.Count; i++)
            {
                Console.BackgroundColor = (ConsoleColor)rnd.Next(1, 5);
                Console.Write("album#" + i.ToString());
                Console.Write(" name: " + albums[i].Title);
                Console.Write(", ID: " + albums[i].Id.ToString());
                Console.WriteLine(", photos count: " + albums[i].Size.ToString());
            }
            Console.BackgroundColor = 0;
        }

        public void DisplayException(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex is VkNet.Exception.TooMuchOfTheSameTypeOfActionException)
                Console.WriteLine(": try changing ip or wait for 24 hours");
            if (ex is System.Net.WebException)
                Console.WriteLine(": network error retrying in 5 seconds...");
        }
    }
}
