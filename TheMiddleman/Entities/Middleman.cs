namespace TheMiddleman.Entity
{
    public class Middleman
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Company { get; }
        public double AccountBalance { get; set; }
        public int MaxStorageCapacity { get; set; } = 100;
        public Dictionary<Product, int> Warehouse { get; set; } = new Dictionary<Product, int>();
        public double PreviousDayBalance { get; set; }
        public double DailyExpenses { get; set; }
        public double DailyEarnings { get; set; }
        public double DailyStorageCosts { get; set; }
        public bool BankruptcyNotified { get; set; } = false;
        public Loan? CurrentLoan { get; set; }

        public Middleman(string name, string company, double accountBalance)
        {
            Name = name;
            Company = company;
            AccountBalance = accountBalance;
        }
    }

    public class Loan
    {
        public double Amount { get; set; }
        public double InterestRate { get; set; }
        public double AmountDue { get; set; }
        public int DueDay { get; set; }
    }
}