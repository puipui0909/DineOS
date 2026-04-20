using DineOS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await _service.GetSummaryAsync();
            return Ok(data);
        }

        [HttpGet("revenue-monthly")]
        public async Task<IActionResult> GetRevenueMonthly([FromQuery] int year)
        {
            var data = await _service.GetRevenueMonthlyAsync(year);
            return Ok(data);
        }
    }
}
