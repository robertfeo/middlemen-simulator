using TheMiddleman.Entity;

namespace TheMiddleman.DataAccess
{
    public class MiddlemanRepository : IMiddlemanRepository
    {
        private List<Middleman> _middlemen = new List<Middleman>();
        private List<Middleman> _bankruptMiddlemen = new List<Middleman>();

        public int NumberOfParticipatingMiddlemen()
        {
            return _middlemen.Count;
        }

        public void AddMiddleman(Middleman middleman)
        {
            _middlemen.Add(middleman);
        }

        public void AddBankruptMiddleman(Middleman middleman)
        {
            _bankruptMiddlemen.Add(middleman);
        }

        public List<Middleman> RetrieveMiddlemen()
        {
            return _middlemen;
        }

        public List<Middleman> RetrieveBankruptMiddlemen()
        {
            return _bankruptMiddlemen;
        }

        public List<Product> GetOwnedProducts(Middleman middleman)
        {
            return middleman.Warehouse.Keys.ToList();
        }

        public Product GetProductByID(int id)
        {
            if (id < 0)
            {
                throw new ProductNotFoundException($"Produkt mit der Id {id} nicht gefunden.");
            }
            if (_middlemen.Select(m => m.Warehouse.ElementAt(id - 1).Key).Any())
            {
                try
                {
                    return _middlemen.Select(m => m.Warehouse.ElementAt(id - 1).Key).Single();
                }
                catch (InvalidOperationException)
                {
                    throw new ProductNotFoundException($"Produkt mit der Id {id} nicht gefunden.");
                }
            }
            else
            {
                throw new ArgumentNullException($"Produkt mit der Id {id} nicht gefunden.");
            }
        }
    }
}