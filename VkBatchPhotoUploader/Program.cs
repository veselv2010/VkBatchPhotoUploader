using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Text;
using System.Windows.Forms;
using VkNet;
using VkNet.Model;
using Newtonsoft.Json.Linq;

namespace VkBatchPhotoUploader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                AccessToken = GetAccessToken().Result
            });

            string[] files = GetFolderFiles();
            long albumId = AlbumSelector(api);
            UploadPhotos(api, albumId, files); //сделать счетчик
            Console.ReadKey();
        }

        async private static Task<string> GetAccessToken()
        {
            string url = "https://oauth.vk.com/authorize?client_id=7096347&display=page&scope=photos&redirect_uri=http://blank.org/&response_type=code&v=5.103";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });

                Console.Write("code=");
                string code = Console.ReadLine();

                var tokenParamsDict = new Dictionary<string, string>();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.vk.com/access_token");

                tokenParamsDict.Add("client_id", "7096347");
                tokenParamsDict.Add("client_secret", "dfem1KnHOVrDN21VHckc");
                tokenParamsDict.Add("redirect_uri", "http://blank.org/");
                tokenParamsDict.Add("code", code);

                request.Content = new FormUrlEncodedContent(tokenParamsDict);

                using (var client = new HttpClient())
                {
                    string response = await client.SendAsync(request)
                    .Result.Content.ReadAsStringAsync();

                    return JObject.Parse(response)
                        .GetValue("access_token").ToString();
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static string[] GetFolderFiles()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return Directory.GetFiles(fbd.SelectedPath);
                }
                else
                {
                    return null;
                }
            }
        }

        private static long AlbumSelector(VkApi api)
        {
            var albums = api.Photo.GetAlbums(new VkNet.Model.RequestParams.PhotoGetAlbumsParams { });
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
            Console.Write("# of desired album: ");

            if(int.TryParse(Console.ReadLine(), out int id))
            {
                return albums[id].Id;
            }

            return 0;
        }

        private static void UploadPhotos(VkApi api, long albumId, string[] files)
        {
            var progressBar = new ProgressBar();
            for (int i = 0; i < files.Length; i++)
            {
                Console.Title = "Uploading: " + files[i];
                var uploadServer = api.Photo.GetUploadServer(albumId);

                using (var wc = new WebClient())
                {
                    var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, files[i]));
                    api.Photo.Save(new VkNet.Model.RequestParams.PhotoSaveParams
                    {
                        SaveFileResponse = responseFile,
                        AlbumId = albumId
                    });
                }
                double progress = (double)(i+1) / files.Length;
                progressBar.Report(progress);
            }
        }
    }
}
