using Microsoft.AspNetCore.Mvc;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;

namespace YoutubeDownloader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeDownloadController: ControllerBase
    {
        private readonly YoutubeDL ytdl;
        public YoutubeDownloadController() {
            ytdl = new YoutubeDL();
            string baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));

            ytdl.YoutubeDLPath = Path.Combine(baseDir, "executables", "yt-dlp.exe");
            ytdl.FFmpegPath = Path.Combine(baseDir, "executables", "ffmpeg.exe");
            ytdl.OutputFolder = Path.Combine(baseDir, "downloads");
        }

        [HttpGet]
        [Route("downloadVideo/{videoUrl}")]
        public async Task<IActionResult> GetVideos(string videoUrl)
        {
            try
            {
                RunResult<string> videoFile = await ytdl.RunVideoDownload(Uri.UnescapeDataString(videoUrl));

                if(videoFile.ErrorOutput.Length > 0)
                {
                    throw new UriFormatException(string.Join(", ", videoFile.ErrorOutput));
                }
                return Ok($"Downloaded `{Path.GetFileName(videoFile.Data)}` at `{ytdl.OutputFolder}`");
            }
            catch (Exception ex) { 
                return BadRequest( new
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
                IEnumerable<Task> downloadTask = videoUrls.Select(async videoUrl =>
                {
                    RunResult<string> res = await ytdl.RunVideoDownload(Uri.UnescapeDataString(videoUrl));
                    if (res.ErrorOutput.Length > 0)
                    {
                        throw new UriFormatException(string.Join(", ", res.ErrorOutput));
                    }
                });
                await Task.WhenAll(downloadTask);

                return Ok($"Downloaded at `{ytdl.OutputFolder}`");
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
