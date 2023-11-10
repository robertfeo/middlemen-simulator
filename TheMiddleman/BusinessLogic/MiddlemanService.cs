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
        int totalCost = quantity * selectedProduct.BasePrice;
        int totalQuantityAfterPurchase = middleman.Warehouse.Values.Sum() + quantity;
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

    public void Sale(Middleman middleman, Product product, int quantity, out string errorLog)
    {
        errorLog = "";
        if (!middleman.Warehouse.ContainsKey(product))
        {
            errorLog = "Produkt ist nicht im Lager vorhanden.";
            return;
        }
        if (middleman.Warehouse[product] < quantity)
        {
            errorLog = $"Nicht genug {product.Name} im Lager. Verfügbar: {middleman.Warehouse[product]} Einheiten.";
            return;
        }
        middleman.Warehouse[product] -= quantity;
        if (middleman.Warehouse[product] == 0)
        {
            middleman.Warehouse.Remove(product);
        }
        middleman.AccountBalance += quantity * product.SellingPrice;
    }

    public void IncreaseWarehouseCapacity(Middleman middleman, int amount)
    {
        middleman.MaxStorageCapacity += amount;
    }

    public int CalculateStorageCosts(Middleman middleman)
    {
        int occupiedUnits = middleman.Warehouse.Values.Sum();
        int emptyUnits = middleman.MaxStorageCapacity - occupiedUnits;
        return occupiedUnits * 5 + emptyUnits * 1;
    }

    public void DeductStorageCosts(Middleman middleman)
    {
        int storageCosts = CalculateStorageCosts(middleman);
        middleman.AccountBalance -= storageCosts;
    }

    public List<Middleman> RetrieveAllMiddlemen()
    {
        return _middlemanRepository.RetrieveAllMiddlemen();
    }
}
