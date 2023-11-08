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

        public Middleman RetrieveMiddlemanById(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("Id darf nicht negativ sein.");
            }
            else if (id >= _middlemen.Count)
            {
                throw new ArgumentException("Id darf nicht größer als die Anzahl der Middlemen sein.");
            }
            else
            {
                return _middlemen[id];
            }
        }
    }
}
