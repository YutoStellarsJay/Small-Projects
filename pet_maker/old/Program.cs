List<string> ourAnimals = new List<string> { "0,Dragon,12,Sense of humor,Fighting,Snowflake", "1,bunny,9,CALM,Cuddles,Bunnila", "2 ,Panga Wanga,3,Many scars,Ruuude,Poopada" };
string? input;
int select;
bool exit = false;
string[] reads;

Console.Write("Welcome to the pet adopting app!, Enter read, add or exit:");

do
{
    input = Console.ReadLine();
    switch (input!.ToLower())
    {
        case "read":
            Console.WriteLine("Pick a pet by id:");
            foreach (string read in ourAnimals)
            {
                reads = read.Split(",");
                Console.WriteLine("[ Name:" + reads[5] + " ID:" + reads[0] + " ]");
            }
            Console.WriteLine("Pick a pet to look by id:");
            select = 0;
            int.TryParse(Console.ReadLine(), out select);
            reads = ourAnimals[select].Split(",");
            Console.WriteLine("The pet " + reads[5] + " is " + reads[2] + " years old who is shown with/as a " + reads[3] + ", they are a " + reads[4] + " type of " + reads[1] + " (ID:" + reads[0] + ")");
            break;
        case "add":
            string adding = "";
            adding += ourAnimals.Count + ",";
            Console.Write("What creature are they? "); adding += Console.ReadLine(); adding += ",";
            Console.Write("How old are they? "); adding += Console.ReadLine(); adding += ",";
            Console.Write("Give a few words to describe them? (Near 5) "); adding += Console.ReadLine(); adding += ",";
            Console.Write("What do they like? "); adding += Console.ReadLine(); adding += ",";
            Console.Write("What is their current nickname? "); adding += Console.ReadLine();
            ourAnimals.Add(adding);
            break;
        case "exit":
            exit = true;
            break;
        default:
            Console.WriteLine("Improper Input, please try again");
            break;
    }
    Console.Write("Enter the choices read, add or exit:");
} while (exit == false);
