using DineOS.Application.Common.Interfaces;
using DineOS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("{orderId}/pay")]
        public async Task<IActionResult> Pay(Guid orderId, [FromBody] CreatePaymentRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            try
            {
                await _paymentService.PayAsync(orderId, request.Method);
                return Ok(new
                {
                    orderId,
                    status = "PAID"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var payment = await _paymentService.GetByOrderIdAsync(orderId);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetById(Guid paymentId)
        {
            var payment = await _paymentService.GetByIdAsync(paymentId);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }
    }

    public class CreatePaymentRequest
    {
        public PaymentMethod Method { get; set; }
    }
}