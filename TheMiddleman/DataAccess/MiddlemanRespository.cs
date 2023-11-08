using TheMiddleman.Entity;
namespace TheMiddleman.DataAccess

{
    public class MiddlemanRespository : IMiddlemanRespository
    {
        private List<Middleman> middlemen = new List<Middleman>();

        public int NumberOfParticipatingMiddlemen()
        {
            return middlemen.Count;
        }

        public void AddMiddleman(Middleman middleman)
        {
            middlemen.Add(middleman);
        }

        public List<Middleman> RetrieveAllMiddlemen()
        {
            return middlemen;
        }

        public Middleman RetrieveMiddlemanById(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("Id darf nicht negativ sein.");
            }
            else if (id >= middlemen.Count)
            {
                throw new ArgumentException("Id darf nicht größer als die Anzahl der Middlemen sein.");
            }
            else
            {
                return middlemen[id];
            }
        }
    }
}
