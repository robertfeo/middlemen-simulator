using System.Collections.Generic;  // Add this line

namespace TheMiddleman.Entity
{
    public class Middleman
    {
        public Middleman(string name, string company, int accountBalance)
        {
            Name = name;
            Company = company;
            AccountBalance = accountBalance;
        }

        public string? Name { get; }
        public string? Company { get; }
        public int AccountBalance { get; set; }
        public Dictionary<Product, int> OwnedProducts { get; set; } = new Dictionary<Product, int>();
    }
}