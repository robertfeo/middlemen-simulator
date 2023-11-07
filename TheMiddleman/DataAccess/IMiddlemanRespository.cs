using TheMiddleman.Entity;

namespace TheMiddleman.DataAccess
{
    public interface IMiddlemanRespository
    {
        List<Middleman> GetAllMiddlemen();
        int GetAmountOfMiddlemen();
        void AddMiddleman(Middleman middleman);
    }
}