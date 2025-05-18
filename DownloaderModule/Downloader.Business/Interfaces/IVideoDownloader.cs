using Downloader.Model.Model;
using YoutubeDLSharp.Metadata;

namespace Downloader.Business.Interfaces
{
    public interface IVideoDownloader
    {
        public Task<VideoFile> VideoDownload(string videoUrl);

        public Task<VideoFile> VideosDownload(List<string> videoUrls);

        public Task<VideoData> VideoDetails(string videoUrl);
    }
}
