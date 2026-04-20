using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using DineOS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly DineOSDbContext _context;

        public RestaurantController(DineOSDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Restaurants.ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRestaurantRequest request)
        {
            var restaurant = new Restaurant(request.Name, request.Address);

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            return Ok(restaurant);
        }
    }
}
