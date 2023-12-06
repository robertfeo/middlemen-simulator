using TheMiddleman.Entity;

namespace TheMiddleman.DataAccess
{
    public interface IMiddlemanRepository
    {
        List<Middleman> RetrieveMiddlemen();
        List<Middleman> RetrieveBankruptMiddlemen();
        int NumberOfParticipatingMiddlemen();
        void AddMiddleman(Middleman middleman);
        void AddBankruptMiddleman(Middleman middleman);
        List<Product> GetOwnedProducts(Middleman middleman);
        Product GetProductByID(int id);
    }
}