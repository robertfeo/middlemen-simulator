using System.Collections.Generic;

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
        public static int MaxStorageCapacity { get; set; } = 100;
        public Dictionary<Product, int> Warehouse { get; set; } = new Dictionary<Product, int>();
    }
}
