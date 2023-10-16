namespace TheMiddleman.Entity
{
    ic class Product
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }

        public int Durability { get; set; }
    }
}