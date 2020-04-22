using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VkNet.Abstractions;

namespace VkBatchPhotoUploader
{
    class VkPhotoUploader
    {
        private IDialogManager dialogManager { get; }
        private IVkApi api { get; }
        public VkPhotoUploader(IVkApi api, IDialogManager dialogManager)
        {
            this.api = api;
        }
        public string[] GetFolderFiles()
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
        public long AlbumSelector()
        {
            var albums = this.api.Photo.GetAlbums(new VkNet.Model.RequestParams.PhotoGetAlbumsParams { });

            dialogManager.DisplayMessage(albums);
            dialogManager.DisplayMessage("# of desired album: ");

            if (int.TryParse(dialogManager.Ask(), out int id))
            {
                return (id >= 0 && id < albums.Count) ?
                    albums[id].Id :
                    AlbumSelector();
            }
            else
            {
                return AlbumSelector();
            }
        }

        public void UploadPhotos(long albumId, string[] photos)
        {
            using var progressBar = new ProgressBar();
            using var wc = new WebClient();
            for (int i = 0; i < photos.Length; i++)
            {
                Console.Title = "Uploading: " + photos[i];
                var uploadServer = api.Photo.GetUploadServer(albumId);
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, photos[i]));
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
                    progressBar.Dispose();
                    dialogManager.DisplayMessage(ex);
                    return;
                }
                catch (WebException ex)
                {
                    dialogManager.DisplayMessage(ex);
                    Thread.Sleep(5000);
                    i--;
                    continue;
                }

                double progress = (double)(i + 1) / photos.Length;
                progressBar.Report(progress);
            }
        }
    }
}
