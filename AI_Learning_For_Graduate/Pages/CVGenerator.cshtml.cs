using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AI_Learning_For_Graduate.Pages
{
    public class CVGeneratorModel : PageModel
    {
        [BindProperty] public string Prompt { get; set; }
        [BindProperty] public string FullName { get; set; }
        [BindProperty] public string Education { get; set; }
        [BindProperty] public string Experience { get; set; }
        [BindProperty] public string Skills { get; set; }

        private readonly string[] Labels = { "Họ tên", "Học vấn", "Kinh nghiệm", "Kỹ năng" };

        public async Task<IActionResult> OnPostAsync(string mode)
        {
            if (mode == "ai" && !string.IsNullOrWhiteSpace(Prompt))
            {
                string finalPrompt = BuildStructuredPrompt(Prompt);

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"D:\Ollama\ollama.exe",
                        Arguments = $"run mistral \"{finalPrompt}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.UTF8
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();

                Console.OutputEncoding = Encoding.UTF8;
                Console.WriteLine("=== AI Output ===");
                Console.WriteLine(output);

                if (IsJson(output))
                {
                    try
                    {
                        var doc = JsonDocument.Parse(output);
                        FullName = doc.RootElement.GetProperty("fullName").GetString();
                        Education = doc.RootElement.GetProperty("education").GetString();
                        Experience = doc.RootElement.GetProperty("experience").GetString();
                        Skills = doc.RootElement.GetProperty("skills").GetString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi parse JSON: " + ex.Message);
                    }
                }
                else
                {
                    FullName = ExtractBlock(output, "Họ tên");
                    Education = ExtractBlock(output, "Học vấn");
                    Experience = ExtractBlock(output, "Kinh nghiệm");
                    Skills = ExtractBlock(output, "Kỹ năng");
                }
            }

            return Page();
        }

        private string BuildStructuredPrompt(string userPrompt)
        {
            return $@"{userPrompt}

Trả về kết quả dưới dạng văn bản với các phần sau:
Họ tên: [Tên đầy đủ]
Học vấn: [Thông tin học vấn]
Kinh nghiệm: [Thông tin kinh nghiệm làm việc]
Kỹ năng: [Danh sách kỹ năng chuyên môn]

Chỉ trả về đúng các phần trên, không thêm mô tả hoặc định dạng khác.";
        }

        private bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) ||
                   (input.StartsWith("[") && input.EndsWith("]"));
        }

        private string ExtractBlock(string text, string label)
        {
            var labelIndex = Array.IndexOf(Labels, label);
            var nextLabel = labelIndex < Labels.Length - 1 ? Labels[labelIndex + 1] : null;

            var startPattern = Regex.Escape(label) + @"\s*:";
            var endPattern = nextLabel != null ? Regex.Escape(nextLabel) + @"\s*:" : "$";

            var pattern = $"{startPattern}(.*?){endPattern}";
            var match = Regex.Match(text, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }
    }
}