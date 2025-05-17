using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;
using YoutubeDownloader.Service.exceptions;

namespace YoutubeDownloader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoDownloadController: ControllerBase
    {
        private readonly YoutubeDL ytdl;
        public VideoDownloadController()
        {
            ytdl = new YoutubeDL();
            string baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));

            ytdl.YoutubeDLPath = Path.Combine(baseDir, "executables", "yt-dlp.exe");
            ytdl.FFmpegPath = Path.Combine(baseDir, "executables", "ffmpeg.exe");
            ytdl.OutputFolder = Path.Combine(baseDir, "downloads");
        }

        [HttpGet]
        [Route("downloadVideo/{videoUrl}")]
        public async Task<IActionResult> GetVideo(string videoUrl)
        {
            try
            {
                var defaultYoutubePath = ytdl.OutputFolder;
                ytdl.OutputFolder = Path.Combine(ytdl.OutputFolder, DateTime.Now.ToString("yyyyMMddHHmmss"));
                Directory.CreateDirectory(ytdl.OutputFolder);
                var options = new OptionSet()
                {
                    RestrictFilenames = true
                };
                RunResult<string> videoFile = await ytdl.RunVideoDownload(Uri.UnescapeDataString(videoUrl), overrideOptions: options);

                if(videoFile.ErrorOutput.Any(error => error.Contains("error", StringComparison.OrdinalIgnoreCase)))
                {
                    throw new UrlProcessingException(string.Join(", ", videoFile.ErrorOutput));
                }

                var fileName = Path.GetFileName(videoFile.Data);
                var fileBytes = System.IO.File.ReadAllBytes(videoFile.Data);
                if (Directory.Exists(ytdl.OutputFolder))
                {
                    Directory.Delete(ytdl.OutputFolder, recursive: true);
                }
                ytdl.OutputFolder = defaultYoutubePath;
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost]
        [Route("downloadVideos")]
        public async Task<IActionResult> GetVideos([FromBody] List<string> videoUrls)
        {
            try
            {
                var defaultYoutubePath = ytdl.OutputFolder;
                ytdl.OutputFolder = Path.Combine(ytdl.OutputFolder, DateTime.Now.ToString("yyyyMMddHHmmss"));
                Directory.CreateDirectory(ytdl.OutputFolder);
                List<string> downloadFilePaths = new List<string>();
                var options = new OptionSet()
                {
                    RestrictFilenames = true
                };
                IEnumerable<Task> downloadTask = videoUrls.Select(async videoUrl =>
                {
                    RunResult<string> videoFile = await ytdl.RunVideoDownload(Uri.UnescapeDataString(videoUrl), overrideOptions: options);
                    if (videoFile.ErrorOutput.Any(error => error.Contains("error", StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new UrlProcessingException(string.Join(", ", videoFile.ErrorOutput));
                    }
                    downloadFilePaths.Add(videoFile.Data);
                });
                await Task.WhenAll(downloadTask);
                string zipFileName = "Videos" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                string zipPath = Path.Combine(ytdl.OutputFolder, zipFileName);
                using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var filePath in downloadFilePaths)
                    {
                        zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                    }
                }

                var zipBytes = System.IO.File.ReadAllBytes(zipPath);

                if (Directory.Exists(ytdl.OutputFolder))
                {
                    Directory.Delete(ytdl.OutputFolder, recursive: true);
                }
                ytdl.YoutubeDLPath = defaultYoutubePath;
                return File(zipBytes, "application/zip", zipFileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        [Route("fetchVideoDetails/{videoUrl}")]
        public async Task<IActionResult> FetchVideoDetails(string videoUrl)
        {
            try
            {
                RunResult<VideoData> videoDetails = await ytdl.RunVideoDataFetch(Uri.UnescapeDataString(videoUrl));
                if (videoDetails.ErrorOutput.Length > 0)
                {
                    throw new UriFormatException(string.Join(", ", videoDetails.ErrorOutput));
                }
                return Ok(videoDetails.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
