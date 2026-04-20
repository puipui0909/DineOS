using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
    }
}
