using TheMiddleman.DataAccess;
using TheMiddleman.Entity;

public class MiddlemanService
{
    private readonly IMiddlemanRepository _middlemanRepository;

    public MiddlemanService()
    {
        _middlemanRepository = new MiddlemanRepository();
    }

    public Middleman CreateAndStoreMiddleman(string name, string company, int initialBalance)
    {
        Middleman middleman = new Middleman(name, company, initialBalance);
        _middlemanRepository.AddMiddleman(middleman);
        return middleman;
    }

    public void PurchaseProduct(Middleman middleman, Product selectedProduct, int quantity)
    {
        var totalCost = quantity * selectedProduct.BasePrice;
        var totalQuantityAfterPurchase = middleman.Warehouse.Values.Sum() + quantity;
        if (totalQuantityAfterPurchase > middleman.MaxStorageCapacity)
        {
            throw new WarehouseCapacityExceededException("Kein Platz mehr im Lager. Verfügbarer Platz: " + (middleman.MaxStorageCapacity - middleman.Warehouse.Values.Sum()) + " Einheiten.");
        }
        if (middleman.AccountBalance < totalCost)
        {
            throw new InsufficientFundsException("Nicht genügend Geld vorhanden. Verfügbares Guthaben: $" + middleman.AccountBalance);
        }
        selectedProduct.AvailableQuantity -= quantity;
        middleman.AccountBalance -= totalCost;
        UpdateWarehouse(middleman, selectedProduct, quantity);
    }

    private void UpdateWarehouse(Middleman middleman, Product product, int quantity)
    {
        if (middleman.Warehouse.ContainsKey(product))
        {
            middleman.Warehouse[product] += quantity;
        }
        else
        {
            middleman.Warehouse.Add(product, quantity);
        }
    }

    public void SellProduct(Middleman middleman, Product product, int quantity)
    {
        if (!middleman.Warehouse.ContainsKey(product) || middleman.Warehouse[product] < quantity)
        {
            throw new ProductNotAvailableException("Nicht genügend Produkt zum Verkauf vorhanden.");
        }
        middleman.Warehouse[product] -= quantity;
        if (middleman.Warehouse[product] == 0)
        {
            middleman.Warehouse.Remove(product);
        }
        middleman.AccountBalance += quantity * product.SellingPrice;
    }

    public void IncreaseWarehouseCapacity(Middleman middleman, int increaseAmount)
    {
        int costForIncrease = increaseAmount * 50;
        if (middleman.AccountBalance < costForIncrease)
        {
            throw new InsufficientFundsException($"Nicht genügend Geld für die Erweiterung des Lagers vorhanden.");
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
        if (middleman.AccountBalance <= 0)
        {
            throw new InsufficientFundsException("Nicht genügend Geld für die Lagerkosten vorhanden.");
        }
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

    public void AddBankruptMiddleman(Middleman middleman)
    {
        _middlemanRepository.RetrieveMiddlemen().Remove(middleman);
        _middlemanRepository.AddBankruptMiddleman(middleman);
    }
}