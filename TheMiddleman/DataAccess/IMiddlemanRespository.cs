using TheMiddleman.Entity;

namespace TheMiddleman.DataAccess
{
    public interface IMiddlemanRespository
    {
        List<Middleman> RetrieveAllMiddlemen();
        List<Middleman> RetrieveBankruptMiddlemen();
        int NumberOfParticipatingMiddlemen();
        void AddMiddleman(Middleman middleman);
    }
}