using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VkNet;

namespace VkBatchPhotoUploader
{
    class VkPhotoUploader
    {
        private ConsoleDialogManager dialogManager { get; }
        private VkApi api { get; }
        private string[] files { get; set; }
        private long albumId { get; set; }
        public VkPhotoUploader(VkApi api, ConsoleDialogManager dialogManager)
        {
            this.api = api;
            this.dialogManager = dialogManager;
            this.files = GetFolderFiles();
            this.albumId = AlbumSelector(this.api);
            UploadPhotos(this.api, this.albumId, this.files);
        }
        private string[] GetFolderFiles()
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
                    throw new IOException();
                }
            }
        }
        private long AlbumSelector(VkApi api)
        {
            var albums = api.Photo.GetAlbums(new VkNet.Model.RequestParams.PhotoGetAlbumsParams { });

            dialogManager.DisplayAlbumRequest(albums);
            string albumIndex = dialogManager.AskAlbum();

            if (int.TryParse(albumIndex, out int id))
            {
                return (id >= 0 && id < albums.Count) ?
                    albums[id].Id :
                    AlbumSelector(api);
            }
            else
            {
                return AlbumSelector(api);
            }
        }

        private void UploadPhotos(VkApi api, long albumId, string[] files)
        {
            using var progressBar = new ProgressBar();
            using var wc = new WebClient();
            for (int i = 0; i < files.Length; i++)
            {
                Console.Title = "Uploading: " + files[i];
                var uploadServer = api.Photo.GetUploadServer(albumId);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, files[i]));
                try
                {
                    api.Photo.Save(new VkNet.Model.RequestParams.PhotoSaveParams
                    {
                        SaveFileResponse = responseFile,
                        AlbumId = albumId
                    });
                }
                catch (VkNet.Exception.TooMuchOfTheSameTypeOfActionException ex)
                {
                    dialogManager.DisplayException(ex);
                    progressBar.Dispose();
                    return;
                }
                catch (WebException ex)
                {
                    dialogManager.DisplayException(ex);
                    Thread.Sleep(5000);
                    i--;
                    continue;
                }

                double progress = (double)(i + 1) / files.Length;
                progressBar.Report(progress);
            }
        }
    }
}
