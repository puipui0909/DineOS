using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class CreateMenuItemRequest
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class UpdateMenuItemRequest
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }
}
