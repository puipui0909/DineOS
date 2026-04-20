using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class CreateMultipleTableDto
    {
        public int Quantity { get; set; }
        public Guid RestaurantId { get; set; }
    }
}
