using System.Runtime.Serialization;

namespace TheMiddleman.DataAccess
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _filePath;
        private List<Product> _products = null!;

        public ProductRepository()
        {
            _filePath = "./produkte.yml";
        }

        public string ReadProductName(string line)
        {
            return line.Substring(8);
        }

        public int ReadProductDurability(string line)
        {
            return int.Parse(line.Substring(14));
        }

        public int ReadProductBasePrice(string line)
        {
            return int.Parse(line.Substring(13));
        }

        public Product CreateProduct(int id, string name, int durability)
        {
            return new Product { Id = id, Name = name, Durability = durability };
        }

        public List<Product> CreateProducts()
        {
            string[] lines = File.ReadAllLines(_filePath);
            _products = new List<Product>();
            Product? currentProduct = null;
            int idCounter = 1;
            foreach (var line in lines)
            {
                if (line.StartsWith("- name: "))
                {
                    currentProduct = ProcessProductName(line, ref idCounter);
                }
                else if (line.StartsWith("  durability: "))
                {
                    ProcessProductDurability(line, currentProduct);
                }
                else if (line.StartsWith("  baseprice: "))
                {
                    ProcessProductBasePrice(line, currentProduct);
                }
                else if (line.StartsWith("  minProductionRate: "))
                {
                    ProcessMinProductionRate(line, currentProduct);
                }
                else if (line.StartsWith("  maxProductionRate: "))
                {
                    ProcessMaxProductionRate(line, currentProduct);
                }
            }
            return _products;
        }

        private void ProcessMaxProductionRate(string line, Product? currentProduct)
        {
            int maxProductionRate = int.Parse(line.Substring(20));
            if (currentProduct != null)
            {
                currentProduct.MaxProductionRate = maxProductionRate;
            }
        }

        private void ProcessMinProductionRate(string line, Product? currentProduct)
        {
            int minProductionRate = int.Parse(line.Substring(20));
            if (currentProduct != null)
            {
                currentProduct.MinProductionRate = minProductionRate;
            }
        }

        private Product? ProcessProductName(string line, ref int idCounter)
        {
            string name = ReadProductName(line);
            return CreateProduct(idCounter++, name, 0);
        }

        private void ProcessProductDurability(string line, Product? product)
        {
            if (product != null)
            {
                int durability = ReadProductDurability(line);
                product.Durability = durability;
                _products.Add(product);
            }
        }

        private void ProcessProductBasePrice(string line, Product? product)
        {
            if (product != null)
            {
                double basePrice = ReadProductBasePrice(line);
                product.BasePrice = basePrice;
            }
        }

        public List<Product> GetAllProducts()
        {
            return _products;
        }
    }
}