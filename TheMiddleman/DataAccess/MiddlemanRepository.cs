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

        public List<Middleman> RetrieveAllMiddlemen()
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
    }
}