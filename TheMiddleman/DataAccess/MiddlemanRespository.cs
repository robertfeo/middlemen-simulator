using TheMiddleman.Entity;
namespace TheMiddleman.DataAccess

{
    public class MiddlemanRespository : IMiddlemanRespository
    {
        private List<Middleman> _middlemen = new List<Middleman>();

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
    }
}
