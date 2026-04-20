using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/customer/orders")]
    public class CustomerOrderController : ControllerBase
    {
        private readonly ICustomerOrderService _customerOrderService;

        public CustomerOrderController(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        // 1. Lấy order (KHÔNG create)
        [HttpGet("by-table/{tableId}")]
        public async Task<IActionResult> GetByTable(Guid tableId)
        {
            var order = await _customerOrderService.GetByTableAsync(tableId);
            if (order == null)
                return NoContent();
            return Ok(order);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _customerOrderService.CreateOrderAsync(request);
            return Ok(result);
        }
    }
}
