using System.Collections.Generic;  // Add this line

namespace TheMiddleman.Entity
{
    public class Middleman
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public int AccountBalance { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
