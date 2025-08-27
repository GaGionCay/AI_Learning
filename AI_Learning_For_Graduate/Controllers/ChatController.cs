using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AI_Learning_For_Graduate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"D:\Ollama\ollama.exe",
                    Arguments = $"run mistral \"{request.Message}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8
                }
            };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            return Ok(new { reply = output });
        }
    }

    public class ChatRequest
    {
        public string? Message { get; set; }
    }
}
