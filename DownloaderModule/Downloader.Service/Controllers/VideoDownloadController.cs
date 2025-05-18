using Downloader.Business.Interfaces;
using Downloader.Model.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace YoutubeDownloader.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoDownloadController : ControllerBase
    {
        private readonly IOptions<BaseAppSettings> _appSettings;
        private readonly IWebHostEnvironment _env;
        private readonly IVideoDownloader _videoDownloader;

        public VideoDownloadController(
            IOptions<BaseAppSettings> appsettinngs,
            IWebHostEnvironment env,
            IVideoDownloader videoDownloader
        )
        {
            _appSettings = appsettinngs;
            _env = env;
            _videoDownloader = videoDownloader;
        }

        [HttpGet]
        [Route("downloadVideo/{videoUrl}")]
        public async Task<IActionResult> GetVideo(string videoUrl)
        {
            try
            {
                var videoFile = await _videoDownloader.VideoDownload(videoUrl);
                return File(videoFile.FileBytes, "application/octet-stream", videoFile.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        [Route("downloadVideos")]
        public async Task<IActionResult> GetVideos([FromBody] List<string> videoUrls)
        {
            try
            {
                var videoFile = await _videoDownloader.VideosDownload(videoUrls);
                return File(videoFile.FileBytes, "application/octet-stream", videoFile.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet]
        [Route("fetchVideoDetails/{videoUrl}")]
        public async Task<IActionResult> FetchVideoDetails(string videoUrl)
        {
            try
            {
                var videoData = await _videoDownloader.VideoDetails(videoUrl);
                return Ok(videoData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
