using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class CreateTableDto
    {
        public int TableNumber { get; set; }
        public Guid RestaurantId { get; set; }
    }
}
