using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Downloader.Business;
using Downloader.Service.Model;
using Downloader.Model.Model;

namespace YoutubeDownloader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoDownloadController: ControllerBase
    {
        private readonly IOptions<BaseAppSettings> _appSettings;
        private readonly IWebHostEnvironment _env;
        public VideoDownloadController(IOptions<BaseAppSettings> appsettinngs, IWebHostEnvironment env)
        {
            _appSettings = appsettinngs;
            _env = env;
        }

        [HttpGet]
        [Route("downloadVideo/{videoUrl}")]
        public async Task<IActionResult> GetVideo(string videoUrl)
        {
            try
            {
                var videoDownloader = new VideoDownloader(_appSettings, _env);
                var videoFile = await videoDownloader.VideoDownload(videoUrl);
                return File(videoFile.FileBytes, "application/octet-stream", videoFile.FileName);
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
                var videoDownloader = new VideoDownloader(_appSettings, _env);
                var videoFile = await videoDownloader.VideosDownload(videoUrls);
                return File(videoFile.FileBytes, "application/octet-stream", videoFile.FileName);
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
                var videoDownloader = new VideoDownloader(_appSettings, _env);
                var videoData = await videoDownloader.VideoDetails(videoUrl);
                return Ok(videoData);
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
