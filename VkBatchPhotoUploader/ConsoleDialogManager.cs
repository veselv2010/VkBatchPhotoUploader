using System;
using System.Collections.Generic;
using VkNet.Model;

namespace VkBatchPhotoUploader
{
    interface IDialogManager
    {
        string Ask();
        void DisplayMessage(string message);
        void DisplayMessage(IList<PhotoAlbum> albums);
        void DisplayMessage(Exception ex);
    }

    class ConsoleDialogManager : IDialogManager
    {
        public string Ask()
        {
            return Console.ReadLine();
        }

        public void DisplayMessage(string message)
        {
            Console.Write(message);
        }

        public void DisplayMessage(IList<PhotoAlbum> albums)
        {
            var rnd = new Random();
            for(int i = 0; i < albums.Count; i++)
            {
                var album = albums[i];

                Console.BackgroundColor = (ConsoleColor)rnd.Next(1, 5);
                Console.Write("album#" + i.ToString());
                Console.Write(" name: " + album.Title);
                Console.Write(", ID: " + album.Id.ToString());
                Console.WriteLine(", photos count: " + album.Size.ToString());
            }
            Console.BackgroundColor = 0;
        }

        public void DisplayMessage(Exception ex)
        {
            Console.WriteLine(ex.Message);
            if (ex is VkNet.Exception.TooMuchOfTheSameTypeOfActionException)
                Console.WriteLine(": try changing ip or wait for 24 hours");
            if (ex is System.Net.WebException)
                Console.WriteLine(": network error retrying in 5 seconds...");
        }
    }
}
