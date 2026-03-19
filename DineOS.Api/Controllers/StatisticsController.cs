using DineOS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _service;

        public StatisticsController(StatisticsService service)
        {
            _service = service;
        }

        [HttpGet("revenue/today")]
        public async Task<IActionResult> GetTodayRevenue()
        {
            var revenue = await _service.GetTodayRevenueAsync();

            return Ok(new
            {
                date = DateTime.UtcNow.Date,
                revenue
            });
        }
    }
}
