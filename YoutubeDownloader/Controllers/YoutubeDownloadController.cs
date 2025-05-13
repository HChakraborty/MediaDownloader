using Microsoft.AspNetCore.Mvc;
using YoutubeDLSharp;

namespace YoutubeDownloader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeDownloadController: ControllerBase
    {
        public YoutubeDownloadController() { }

        [HttpGet]
        [Route("downloadVideo/{videoUrl}")]
        public async Task<IActionResult> GetVideos(string videoUrl)
        {
            var ytdl = new YoutubeDL();
            string baseDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));

            ytdl.YoutubeDLPath = Path.Combine(baseDir, "executables", "yt-dlp.exe");
            ytdl.FFmpegPath = Path.Combine(baseDir, "executables", "ffmpeg.exe");
            ytdl.OutputFolder = Path.Combine(baseDir, "downloads");
            try
            {
                var res = await ytdl.RunVideoDownload(Uri.UnescapeDataString(videoUrl));
                if(res.ErrorOutput.Length > 0)
                {
                    throw new UriFormatException(string.Join(", ", res.ErrorOutput));
                }
                return Ok("Downloaded");
            }
            catch (Exception ex) { 
                return BadRequest( new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

    }
}
