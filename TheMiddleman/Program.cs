using TheMiddleman.Entity;

class Program
{
    static void Main()
    {
        List<Middleman> middlemen = new List<Middleman>();
        int currentDay = 1;

        Console.WriteLine("Wieviel Zwischenhändler nehmen teil?");

        int numMiddlemen = int.Parse(Console.ReadLine() ?? "0");

        for (int i = 1; i <= numMiddlemen; i++)
        {
            Console.WriteLine($"Name von Zwischenhändler {i}:");
            string name = Console.ReadLine() ?? "";

            Console.WriteLine($"Name der Firma von {name}:");
            string company = Console.ReadLine() ?? "";

            middlemen.Add(new Middleman { Name = name, Company = company });
        }

        while (true)
        {
            foreach (var middleman in middlemen)
            {
                Console.WriteLine($"{middleman.Name} von {middleman.Company} | Tag {currentDay}");
                Console.WriteLine("b) Runde beenden");
                Console.WriteLine("q) Programm beenden");

                string option = Console.ReadLine() ?? "q";

                if (option == "b")
                {
                    continue;
                }
                else if (option == "q" || option == "")
                {
                    return;
                }
            }

            currentDay++;
        }
    }
}