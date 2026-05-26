using System;
using System.Collections.Generic;
using System.Text;

namespace AltynKazanMAUI.Models
{
    public class Dish
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
