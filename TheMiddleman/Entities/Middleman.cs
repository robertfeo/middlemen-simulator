using System.Collections.Generic;

namespace TheMiddleman.Entity
{
    public class Middleman
    {
        public int Id { get; set; }
        public string? Name { get; }
        public string? Company { get; }
        public int AccountBalance { get; set; }
        public int MaxStorageCapacity { get; set; } = 100;
        public Dictionary<Product, int> Warehouse { get; set; } = new Dictionary<Product, int>();
        public int PreviousDayBalance { get; set; }
        public int DailyExpenses { get; set; }
        public int DailyEarnings { get; set; }
        public int DailyStorageCosts { get; set; }

        public Middleman(string name, string company, int accountBalance)
        {
            Name = name;
            Company = company;
            AccountBalance = accountBalance;
        }
    }
}