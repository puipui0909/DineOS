using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 2️⃣ Thêm món
        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddItem(Guid orderId, [FromBody] AddOrderItemRequest request)
        {
            // Kiểm tra dữ liệu đầu vào cơ bản
            if (request.Quantity <= 0)
                return BadRequest("Số lượng phải lớn hơn 0.");

            await _orderService.AddItemAsync(orderId, request);
            var order = await _orderService.GetByIdAsync(orderId);
            return Ok(order);
        }

        // 4️⃣ Xóa món
        [HttpDelete("{orderId}/items/{orderItemId}")]
        public async Task<IActionResult> RemoveItem(Guid orderId, Guid orderItemId)
        {
            await _orderService.RemoveItemAsync(orderId, orderItemId);
            return Ok();
        }

        [HttpPatch("{orderId}/send-to-kitchen")]
        public async Task<IActionResult> SendToKitchen(Guid orderId)
        {
            await _orderService.SendToKitchenAsync(orderId);
            return Ok("Sent to kitchen");
        }
        // 3️⃣ Close Order
        [HttpPatch("{orderId}/close")]
        public async Task<IActionResult> Close(Guid orderId)
        {
            await _orderService.CloseAsync(orderId);
            return Ok("Order closed successfully");
        }

        // 6️⃣ Hủy order
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> Cancel(Guid orderId)
        {
            try
            {
                await _orderService.CancelAsync(orderId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Lấy order theo id
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // Lấy order theo table
        [HttpGet("table/{tableId}")]
        public async Task<IActionResult> GetByTable(Guid tableId)
        {
            var order = await _orderService.GetOrCreateByTableAsync(tableId);
            return Ok(order);
        }

        // Lấy tất cả order
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            return Ok(orders);
        }

        [HttpGet("{orderId}/bill")]
        public async Task<IActionResult> GetBill(Guid orderId)
        {
            var bill = await _orderService.GetBillAsync(orderId);

            if (bill == null)
                return NotFound();

            return Ok(bill);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string? search, [FromQuery] string? fromDate, [FromQuery] string? toDate)
        {
            DateTime? from = null;
            if (DateTime.TryParse(fromDate, out var f))
                from = f;

            DateTime? to = null;
            if (DateTime.TryParse(toDate, out var t))
                to = t;

            var orders = await _orderService.GetOrderHistoryAsync(search, from, to);
            return Ok(orders);
        }
    }
}
