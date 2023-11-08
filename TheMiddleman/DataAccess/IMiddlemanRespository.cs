using TheMiddleman.Entity;

namespace TheMiddleman.DataAccess
{
    public interface IMiddlemanRespository
    {
        List<Middleman> RetrieveAllMiddlemen();
        int NumberOfParticipatingMiddlemen();
        void AddMiddleman(Middleman middleman);
        Middleman RetrieveMiddlemanById(int v);
    }
}