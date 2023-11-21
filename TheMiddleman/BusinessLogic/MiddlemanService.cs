using TheMiddleman.DataAccess;
using TheMiddleman.Entity;

public class MiddlemanService
{
    private readonly IMiddlemanRespository _middlemanRepository;

    public MiddlemanService()
    {
        _middlemanRepository = new MiddlemanRespository();
    }

    public Middleman CreateAndStoreMiddleman(string name, string company, int initialBalance)
    {
        Middleman middleman = new Middleman(name, company, initialBalance);
        _middlemanRepository.AddMiddleman(middleman);
        return middleman;
    }

    public void Purchase(Middleman middleman, Product selectedProduct, int quantity, out string errorLog)
    {
        errorLog = "";
        var totalCost = quantity * selectedProduct.BasePrice;
        var totalQuantityAfterPurchase = middleman.Warehouse.Values.Sum() + quantity;
        if (totalQuantityAfterPurchase > middleman.MaxStorageCapacity)
        {
            errorLog = "Kein Platz mehr im Lager. Verfügbarer Platz: " + (middleman.MaxStorageCapacity - middleman.Warehouse.Values.Sum()) + " Einheiten.";
            return;
        }
        if (middleman.AccountBalance < totalCost)
        {
            errorLog = "Nicht genügend Geld vorhanden. Verfügbares Guthaben: $" + middleman.AccountBalance;
            return;
        }
        selectedProduct.AvailableQuantity -= quantity;
        middleman.AccountBalance -= totalCost;
        if (middleman.Warehouse.ContainsKey(selectedProduct))
        {
            middleman.Warehouse[selectedProduct] += quantity;
        }
        else
        {
            middleman.Warehouse.Add(selectedProduct, quantity);
        }
    }

    public void Sale(Middleman middleman, Product product, int quantity)
    {
        middleman.Warehouse[product] -= quantity;
        if (middleman.Warehouse[product] == 0)
        {
            middleman.Warehouse.Remove(product);
        }
        middleman.AccountBalance += quantity * product.SellingPrice;
    }

    public void IncreaseWarehouseCapacity(Middleman middleman)
    {
        int increaseAmount;
        if (!int.TryParse(ConsoleUI.GetUserInput(), out increaseAmount) || increaseAmount <= 0)
        {
            ConsoleUI.ShowErrorLog("Vergrößerung des Lagers abgebrochen.");
            return;
        }
        int costForIncrease = increaseAmount * 50;
        if (middleman.AccountBalance < costForIncrease)
        {
            ConsoleUI.ShowErrorLog($"Nicht genug Geld für die Vergrößerung des Lagers vorhanden.\nVerfügbares Guthaben: ${middleman.AccountBalance}");
            return;
        }
        middleman.AccountBalance -= costForIncrease;
        middleman.MaxStorageCapacity += increaseAmount;
    }

    public int CalculateStorageCosts(Middleman middleman)
    {
        var occupiedUnits = middleman.Warehouse.Values.Sum();
        var emptyUnits = middleman.MaxStorageCapacity - occupiedUnits;
        return occupiedUnits * 5 + emptyUnits * 1;
    }

    public void DeductStorageCosts(Middleman middleman)
    {
        var storageCosts = CalculateStorageCosts(middleman);
        middleman.AccountBalance -= storageCosts;
    }

    public List<Middleman> RetrieveAllMiddlemen()
    {
        return _middlemanRepository.RetrieveAllMiddlemen();
    }

    public List<Middleman> RetrieveBankruptMiddlemen()
    {
        return _middlemanRepository.RetrieveBankruptMiddlemen();
    }

    public List<Product> GetOwnedProducts(Middleman middleman)
    {
        return _middlemanRepository.GetOwnedProducts(middleman);
    }
}