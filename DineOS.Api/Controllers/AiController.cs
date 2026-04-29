using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs.Ai;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("suggest")]
        public async Task<IActionResult> Suggest([FromBody] AiRequest request)
        {
            var result = await _aiService.GetSuggestion(request.Message);
            return Ok(result);
        }
    }
}