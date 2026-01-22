using Microsoft.AspNetCore.Mvc;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;

namespace Sky_.Controllers
{
    public class ChatRequest { public string Message { get; set; } = ""; }

    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ChatController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskAI([FromBody] ChatRequest request)
        {
            try
            {
                string? apiKey = _config["AI_Settings:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    return Ok(new { reply = "Lỗi: Chưa tìm thấy API Key trong appsettings.json" });

                var googleAI = new GoogleAI(apiKey);
                var model = googleAI.GenerativeModel(Model.Gemini25Flash);
                var response = await model.GenerateContent(request.Message);

                if (response != null && !string.IsNullOrEmpty(response.Text))
                {
                    return Ok(new { reply = response.Text });
                }

                return Ok(new { reply = "AI không có phản hồi. Hãy thử lại." });
            }
            catch (Exception ex)
            {
                return Ok(new { reply = "Lỗi kết nối AI: " + ex.Message });
            }
        }
    }
}