using TheMiddleman.DataAccess;
using TheMiddleman.Entity;

public class MiddlemanService
{
    private readonly IMiddlemanRepository _middlemanRepository;

    public MiddlemanService()
    {
        _middlemanRepository = new MiddlemanRepository();
    }

    public Middleman CreateAndStoreMiddleman(string name, string company, double initialBalance)
    {
        Middleman middleman = new Middleman(name, company, initialBalance);
        _middlemanRepository.AddMiddleman(middleman);
        return middleman;
    }

    public void PurchaseProduct(Middleman middleman, Product selectedProduct, int quantity)
    {
        double discount = CalculateDiscount(selectedProduct, middleman);
        double discountedPrice = selectedProduct.PurchasePrice * (1 - discount);
        var totalCost = quantity * discountedPrice;
        var totalQuantityAfterPurchase = middleman.Warehouse.Values.Sum() + quantity;
        if (middleman.AccountBalance < totalCost)
        {
            throw new InsufficientFundsException("Nicht genügend Geld vorhanden. Verfügbares Guthaben: " + CurrencyFormatter.FormatPrice(middleman.AccountBalance));
        }
        if (totalQuantityAfterPurchase > middleman.MaxStorageCapacity)
        {
            throw new WarehouseCapacityExceededException("Kein Platz mehr im Lager. Verfügbarer Platz: " + (middleman.MaxStorageCapacity - middleman.Warehouse.Values.Sum()) + " Einheiten.");
        }
        if (selectedProduct.AvailableQuantity < quantity)
        {
            throw new ProductNotAvailableException("Nicht genügend Produkte verfügbar.");
        }
        selectedProduct.AvailableQuantity -= quantity;
        middleman.AccountBalance -= totalCost;
        middleman.DailyExpenses += totalCost;
        UpdateWarehouse(middleman, selectedProduct, quantity);
    }

    private void UpdateWarehouse(Middleman middleman, Product product, int quantity)
    {
        if (middleman.Warehouse.ContainsKey(product))
        {
            middleman.Warehouse[product] += quantity;
        }
        else { middleman.Warehouse.Add(product, quantity); }
    }

    public void SellProduct(Middleman middleman, Product product, int quantity)
    {
        if (!middleman.Warehouse.ContainsKey(product) || middleman.Warehouse[product] < quantity)
        {
            throw new ProductNotAvailableException("Nicht genügend Produkte zum Verkauf vorhanden.");
        }
        middleman.Warehouse[product] -= quantity;
        if (middleman.Warehouse[product] == 0)
        {
            middleman.Warehouse.Remove(product);
        }
        middleman.AccountBalance += quantity * product.SellingPrice;
        middleman.DailyEarnings += quantity * product.SellingPrice;
    }

    public void IncreaseWarehouseCapacity(Middleman middleman, int increaseAmount)
    {
        double costForIncrease = increaseAmount * 50;
        if (middleman.AccountBalance < costForIncrease)
        {
            throw new InsufficientFundsException("Nicht genügend Geld für die Erweiterung des Lagers vorhanden.");
        }
        middleman.AccountBalance -= costForIncrease;
        middleman.MaxStorageCapacity += increaseAmount;
    }

    public double CalculateStorageCosts(Middleman middleman)
    {
        var occupiedUnits = middleman.Warehouse.Values.Sum();
        var emptyUnits = middleman.MaxStorageCapacity - occupiedUnits;
        return occupiedUnits * 5 + emptyUnits * 1;
    }

    public double CalculateDiscount(Product product, Middleman middleman)
    {
        int quantity = middleman.Warehouse.ContainsKey(product) ? middleman.Warehouse[product] : 0;
        if (quantity >= 75) return 0.10;
        if (quantity >= 50) return 0.05;
        if (quantity >= 25) return 0.02;
        return 0;
    }

    public void DeductStorageCosts(Middleman middleman)
    {
        var storageCosts = CalculateStorageCosts(middleman);
        if (middleman.AccountBalance - storageCosts <= 0)
        {
            throw new InsufficientFundsException("Nicht genügend Geld für die Lagerkosten vorhanden.");
        }
        middleman.AccountBalance -= storageCosts;
        middleman.DailyStorageCosts = storageCosts;
    }

    public void ResetDailyReport(Middleman middleman)
    {
        middleman.PreviousDayBalance = middleman.AccountBalance;
        middleman.DailyExpenses = 0;
        middleman.DailyEarnings = 0;
        middleman.DailyStorageCosts = 0;
    }

    public List<Middleman> RetrieveMiddlemen()
    {
        return _middlemanRepository.RetrieveMiddlemen();
    }

    public List<Middleman> RetrieveBankruptMiddlemen()
    {
        return _middlemanRepository.RetrieveBankruptMiddlemen();
    }

    public List<Product> GetOwnedProducts(Middleman middleman)
    {
        return _middlemanRepository.GetOwnedProducts(middleman);
    }

    public Product GetProductByID(int id)
    {
        return _middlemanRepository.GetProductByID(id);
    }

    public bool CheckIfMiddlemanIsLastBankrupted(Middleman middleman)
    {
        var middlemen = _middlemanRepository.RetrieveMiddlemen();
        if (middlemen.Count > 0 && middlemen[middlemen.Count - 1].Equals(middleman))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RegisterNewLoan(int current, Middleman middleman, double amount, double interestRate)
    {
        int dueDay = current + 7;
        double amountDue = amount * (1 + interestRate);
        middleman.CurrentLoan = new Loan
        {
            Amount = amount,
            InterestRate = interestRate,
            AmountDue = amountDue,
            DueDay = dueDay
        };
        middleman.AccountBalance += amount;
    }

    public void HandleLoanRepayment(int currentDay, Middleman middleman)
    {
        if (VerifyLoanDue(middleman, currentDay))
        {
            if (middleman.AccountBalance < middleman.CurrentLoan!.AmountDue)
            {
                throw new InsufficientFundsException("Loan repayment failed. Not enough funds.");
            }
            else
            {
                middleman.AccountBalance -= middleman.CurrentLoan.AmountDue;
                middleman.CurrentLoan = null;
            }
        }
    }

    public bool VerifyLoanDue(Middleman middleman, int currentDay)
    {
        if (middleman.CurrentLoan != null && middleman.CurrentLoan.DueDay == currentDay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}