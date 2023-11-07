using TheMiddleman.Entity;
namespace TheMiddleman.DataAccess

{
    public class MiddlemanRespository : IMiddlemanRespository
    {
        private List<Middleman> middlemen = new List<Middleman>();

        public int GetAmountOfMiddlemen()
        {
            return middlemen.Count;
        }

        public void AddMiddleman(Middleman middleman)
        {
            middlemen.Add(middleman);
        }

        public List<Middleman> GetAllMiddlemen()
        {
            return middlemen;
        }
    }
}
