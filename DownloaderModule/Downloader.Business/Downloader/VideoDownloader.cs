using System.Collections.Concurrent;
using System.IO.Compression;
using Downloader.Business.Interfaces;
using Downloader.Model.exceptions;
using Downloader.Model.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace Downloader.Business
{
    public class VideoDownloader : IVideoDownloader
    {
        private readonly YoutubeDL _ytdl;
        private readonly BaseAppSettings _appSettings;
        private readonly string _baseDir;

        public VideoDownloader(IOptions<BaseAppSettings> appsettings, IWebHostEnvironment env)
        {
            _appSettings = appsettings.Value;
            _baseDir = env.ContentRootPath;
            _ytdl = new YoutubeDL();

            _ytdl.YoutubeDLPath = Path.Combine(
                _baseDir,
                _appSettings.PackageDirectory,
                _appSettings.YtDlpPath
            );
            _ytdl.FFmpegPath = Path.Combine(
                _baseDir,
                _appSettings.PackageDirectory,
                _appSettings.FFmpegPath
            );
            _ytdl.OutputFolder = Path.Combine(
                _baseDir,
                _appSettings.OutputFolder,
                DateTime.Now.ToString("yyyyMMddHHmmss")
            );
        }

        public async Task<VideoFile> VideoDownload(string videoUrl)
        {
            return await GetVideoFromSource(videoUrl);
        }

        public async Task<VideoFile> VideosDownload(List<string> videoUrls)
        {
            return await GetVideosFromSource(videoUrls);
        }

        public async Task<VideoData> VideoDetails(string videoUrl)
        {
            return await GetVideoDetails(videoUrl);
        }

        private async Task<VideoFile> GetVideoFromSource(string videoUrl)
        {
            Directory.CreateDirectory(_ytdl.OutputFolder);
            var options = new OptionSet() { RestrictFilenames = true };
            RunResult<string> videoFile = await _ytdl.RunVideoDownload(
                Uri.UnescapeDataString(videoUrl),
                overrideOptions: options
            );

            if (
                videoFile.ErrorOutput.Any(error =>
                    error.Contains("error", StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                throw new UrlProcessingException(string.Join(", ", videoFile.ErrorOutput));
            }

            var fileName = Path.GetFileName(videoFile.Data);
            var fileBytes = System.IO.File.ReadAllBytes(videoFile.Data);
            DeleteOutputFolder();

            return new VideoFile { FileName = fileName, FileBytes = fileBytes };
        }

        private async Task<VideoFile> GetVideosFromSource(List<string> videoUrls)
        {
            Directory.CreateDirectory(_ytdl.OutputFolder);
            ConcurrentBag<string> downloadFilePaths = new ConcurrentBag<string>();
            var options = new OptionSet() { RestrictFilenames = true };
            IEnumerable<Task> downloadTask = videoUrls.Select(async videoUrl =>
            {
                RunResult<string> videoFile = await _ytdl.RunVideoDownload(
                    Uri.UnescapeDataString(videoUrl),
                    overrideOptions: options
                );
                if (
                    videoFile.ErrorOutput.Any(error =>
                        error.Contains("error", StringComparison.OrdinalIgnoreCase)
                    )
                )
                {
                    throw new UrlProcessingException(string.Join(", ", videoFile.ErrorOutput));
                }
                downloadFilePaths.Add(videoFile.Data);
            });
            await Task.WhenAll(downloadTask).ConfigureAwait(false);

            var (zipBytes, zipFileName) = GenerateZipFile(downloadFilePaths);

            return new VideoFile { FileBytes = zipBytes, FileName = zipFileName };
        }

        private (byte[], string) GenerateZipFile(ConcurrentBag<string> downloadFilePaths)
        {
            string zipFileName = "Videos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
            string zipPath = Path.Combine(_ytdl.OutputFolder, zipFileName);
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (var filePath in downloadFilePaths)
                {
                    zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                }
            }

            var zipBytes = System.IO.File.ReadAllBytes(zipPath);

            DeleteOutputFolder();
            return (zipBytes, zipFileName);
        }

        private async Task<VideoData> GetVideoDetails(string videoUrl)
        {
            RunResult<VideoData> videoDetails = await _ytdl.RunVideoDataFetch(
                Uri.UnescapeDataString(videoUrl)
            );
            if (videoDetails.ErrorOutput.Length > 0)
            {
                throw new UrlProcessingException(string.Join(", ", videoDetails.ErrorOutput));
            }

            return videoDetails.Data;
        }

        private void DeleteOutputFolder()
        {
            try
            {
                if (Directory.Exists(_ytdl.OutputFolder))
                {
                    Directory.Delete(_ytdl.OutputFolder, recursive: true);
                }
            }
            catch (IOException ex)
            {
                throw new Exception(
                    "You are trying to delete a directory or its contents which doesn't exist",
                    ex
                );
            }
        }
    }
}
