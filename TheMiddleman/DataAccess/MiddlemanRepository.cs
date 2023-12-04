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

        public Product FindProductById(int productId)
        {
            try
            {
                return _middlemen.SelectMany(m => m.Warehouse.Keys).Single(p => p.Id == productId);
            }
            catch (InvalidOperationException)
            {
                throw new ProductNotFoundException($"Produkt mit der Id {productId} nicht gefunden.");
            }
        }
    }
}