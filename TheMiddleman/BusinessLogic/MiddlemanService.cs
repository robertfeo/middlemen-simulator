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

    public void ProcessPurchase(Middleman middleman, Product selectedProduct, int quantity)
    {
        int totalCost = quantity * selectedProduct.BasePrice;
        int totalQuantityAfterPurchase = middleman.Warehouse.Values.Sum() + quantity;
        if (totalQuantityAfterPurchase > middleman.MaxStorageCapacity)
        {
            Console.WriteLine("Kein Platz mehr im Lager.");
            return;
        }
        if (middleman.AccountBalance < totalCost)
        {
            Console.WriteLine("Nicht genügend Geld vorhanden.");
            return;
        }
        if (selectedProduct.AvailableQuantity < quantity)
        {
            Console.WriteLine("Nicht genügend Ware vorhanden.");
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
        Console.WriteLine($"Kauf erfolgreich. Neuer Kontostand: ${middleman.AccountBalance}");
    }

    public void ProcessSale(Middleman middleman, Product product, int quantity)
    {
        middleman.Warehouse[product] -= quantity;
        if (middleman.Warehouse[product] == 0)
        {
            middleman.Warehouse.Remove(product);
        }
        middleman.AccountBalance += quantity * product.SellingPrice;
    }

    private bool CanPurchase(Middleman middleman, Product product, int quantity, int totalCost)
    {
        return middleman.AccountBalance >= totalCost && product.AvailableQuantity >= quantity && CanStore(middleman, quantity);
    }

    private bool CanStore(Middleman middleman, int quantity)
    {
        int currentStorage = middleman.Warehouse?.Sum(entry => entry.Value) ?? 0;
        return currentStorage + quantity <= middleman.MaxStorageCapacity;
    }

    public void IncreaseWarehouseCapacity(Middleman middleman, int amount)
    {
        middleman.MaxStorageCapacity += amount;
    }

    public List<Middleman> GetAllMiddlemen()
    {
        return _middlemanRepository.GetAllMiddlemen();
    }
}
