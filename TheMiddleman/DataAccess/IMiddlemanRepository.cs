using TheMiddleman.Entity;

namespace TheMiddleman.DataAccess
{
    public interface IMiddlemanRepository
    {
        List<Middleman> RetrieveAllMiddlemen();
        List<Middleman> RetrieveBankruptMiddlemen();
        int NumberOfParticipatingMiddlemen();
        void AddMiddleman(Middleman middleman);
        List<Product> GetOwnedProducts(Middleman middleman);
    }
}