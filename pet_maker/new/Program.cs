using System;

class PetStore
{
    static HashSet<long> ids = new HashSet<long>();
    public class Pet
    {
        public long id;
        public string species;
        public short age;
        public string summary;
        public string kind;
        public string name;
        public Pet(string specie, short ag, string summar, string kin, string nam)
        {
            long i = 0;
            while (ids.Contains(i))
                i++;
            id = i;
            ids.Add(i);
            species = specie;
            age = ag;
            summary = summar.ToLower();
            kind = kin.ToLower();
            name = nam;
        }
        public string Summary()
        {
            return "The pet " + name + " is " + age + " years old who likes " + summary + ", and is a " + species + " who also enjoys " + kind + " (ID:" + id + ")";
        }
    }
    static string ReadLineWText(string input)
    {
        Console.Write(input);
        return Console.ReadLine() ?? "";
    }
    static void Main()
    {
        List<Pet> ourAnimals = new List<Pet> { new Pet("Dragon", 12, "dancing around flowers", "Fighting", "Snowflake"), new Pet("Bunny", 9, "cuddles", "being CALM", "Bunnila"), new Pet("Poopada", 3, "scraping many scars", "being ruuude ;|", "Panga Wanga") };
        string? input;
        int select;
        bool exit = false;
        do
        {
            Console.Write("Welcome to the pet adopting app!, Enter read, remove, add or exit: ");
            input = Console.ReadLine();
            switch (input!.ToLower())
            {
                case "read":
                    Console.WriteLine("Pick a pet by id:");
                    ourAnimals = ourAnimals.OrderBy(p => p.id).ToList();
                    foreach (Pet p in ourAnimals)
                        Console.WriteLine($"[ Name: {p.name + " ID:" + p.id + " ]",20}");
                    Console.WriteLine("Pick a pet to look by id: ");
                    int.TryParse(Console.ReadLine(), out select);
                    Pet looking = ourAnimals[select];
                    Console.WriteLine(looking.Summary());
                    break;
                case "add":
                    Pet newP = new Pet(ReadLineWText("What species are they? "), short.Parse(ReadLineWText("How old are they? ")), ReadLineWText("Give a summary to describe them? (who likes..) "), ReadLineWText("What one thing do they like? (who also enjoys..) "), ReadLineWText("What is their current nickname? "));
                    ourAnimals.Add(newP);
                    break;
                case "remove":
                    if (ourAnimals.Count >= 1)
                    {
                        Console.WriteLine("Which id to remove?: ");
                        long removal = long.Parse(Console.ReadLine() ?? "");
                        ourAnimals = ourAnimals.Where(p => p.id != removal).ToList();
                        ids.Remove(removal);
                    }
                    else Console.WriteLine("Ooops! There needs to be one pet to keep it working!");
                    break;
                case "exit":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Improper Input, please try again");
                    break;
            }
        } while (exit == false);
    }
}