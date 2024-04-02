using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using System.Security.AccessControl;

namespace Test.API.ASP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("download-with-stream")]
        public async Task<IActionResult> GetTest1()
        {
            // Đường dẫn đến file tĩnh

            string startPath = @"F:\\source\\5-temp\\Test.API.ASP\\Test.API.ASP\\Static\\";

            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("Content-Disposition", "attachment; filename=test.zip");

            using (ZipArchive archive = new ZipArchive(Response.Body, ZipArchiveMode.Create, true))
            {
                string file1 = "pgadmin4-7.4-x64.exe";
                string file2 = "pgadmin4-7 - Copy.4-x64.exe";

                // Thêm file1 vào zip
                using (var fileStream = new FileStream(Path.Combine(startPath, file1), FileMode.Open, FileAccess.Read))
                {
                    var entry = archive.CreateEntry(file1);
                    using (var entryStream = entry.Open())
                    {
                        // Đọc và ghi file theo từng phần nhỏ
                        await fileStream.CopyToAsync(entryStream);
                    }
                }

                // Thêm file2 vào zip
                using (var fileStream = new FileStream(Path.Combine(startPath, file2), FileMode.Open, FileAccess.Read))
                {
                    var entry = archive.CreateEntry(file2);
                    using (var entryStream = entry.Open())
                    {
                        // Đọc và ghi file theo từng phần nhỏ
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }

            return new EmptyResult();
        }

        [HttpGet("download-non-stream")]
        public async Task<FileStreamResult> GetTest()
        {
            // Đường dẫn đến file tĩnh

            string startPath = @"F:\\source\\5-temp\\Test.API.ASP\\Test.API.ASP\\Static\\";

            MemoryStream memoryStreamResult = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    string file1 = "pgadmin4-7.4-x64.exe";
                    string file2 = "pgadmin4-7 - Copy.4-x64.exe";

                    // Thêm file1 vào zip
                    using (var fileStream = new FileStream(Path.Combine(startPath, file1), FileMode.Open, FileAccess.Read))
                    using (var bufferedStream = new BufferedStream(fileStream))
                    {
                        var entry = archive.CreateEntry(file1);
                        using (var entryStream = entry.Open())
                        {
                            // Đọc và ghi file theo từng phần nhỏ
                            bufferedStream.CopyTo(entryStream);
                        }
                    }

                    // Thêm file2 vào zip
                    using (var fileStream = new FileStream(Path.Combine(startPath, file2), FileMode.Open, FileAccess.Read))
                    using (var bufferedStream = new BufferedStream(fileStream))
                    {
                        var entry = archive.CreateEntry(file2);
                        using (var entryStream = entry.Open())
                        {
                            // Đọc và ghi file theo từng phần nhỏ
                            bufferedStream.CopyTo(entryStream);
                        }
                    }
                }

                memoryStream.Position = 0;
                memoryStreamResult = new MemoryStream(memoryStream.ToArray());
            }

            // Trả về FileStreamResult với MemoryStream
            return new FileStreamResult(memoryStreamResult, "application/octet-stream")
            {
                FileDownloadName = "test.zip",
            };
        }
    }
}