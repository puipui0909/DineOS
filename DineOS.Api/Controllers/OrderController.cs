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

        // 1️⃣ Mở bàn
        [HttpPost("open/{tableId}")]
        public async Task<IActionResult> OpenTable(Guid tableId)
        {
            try
            {
                await _orderService.OpenTableAsync(tableId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        // 2️⃣ Thêm món
        [HttpPost("{orderId:guid}/items")]
        public async Task<IActionResult> AddItem(Guid orderId, [FromBody] AddItemRequest request)
        {
            // Kiểm tra dữ liệu đầu vào cơ bản
            if (request.Quantity <= 0)
                return BadRequest("Số lượng phải lớn hơn 0.");

            await _orderService.AddItemAsync(orderId, request.MenuItemId, request.Quantity);

            return Ok(new { Message = "Thêm món vào đơn hàng thành công!" });
        }

        // 4️⃣ Xóa món
        [HttpDelete("{orderId}/items/{orderItemId}")]
        public async Task<IActionResult> RemoveItem(Guid orderItemId)
        {
            await _orderService.RemoveItemAsync(orderItemId);
            return Ok();
        }

        // 3️⃣ Close Order
        [HttpPost("{orderId}/close")]
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
            catch (ApplicationException ex)
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
            var order = await _orderService.GetByTableAsync(tableId);

            if (order == null)
                return NotFound();

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
        public async Task<IActionResult> GetHistory()
        {
            var orders = await _orderService.GetOrderHistoryAsync();
            return Ok(orders);
        }
    }
}
