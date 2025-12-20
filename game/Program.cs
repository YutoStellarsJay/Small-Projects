using System;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

class Sprinketraz
{
    public enum ArmorType : byte
    {
        None = 0,
        ScrapCloth = 1,
        Cloth = 2,
    }

    static Dictionary<byte, int> ArmorResistance = new Dictionary<byte, int>()
    {
        {0,0},{1,1},{2,3}
    };

    public class Armor
    {
        public string ArmorTypeAsString(ArmorType type) => type switch
        {
            ArmorType.None => "None",
            ArmorType.ScrapCloth => "Scrap Cloth",
            ArmorType.Cloth => "Cloth",
            _ => "Unknown"
        };
        public ArmorType[] armors = new ArmorType[4];
        public void WearArmor(int index, ArmorType armor)
        {
            if (index < 0 || index >= armors.Length) return;
            if (OwnedArmor.Contains(armor))
            {
                armors[index] = armor;
            }

        }
        public void AddArmor(ArmorType armor)
        {
            OwnedArmor.Add(armor);
        }
        public void SellArmor(ArmorType armor)
        {
            if (OwnedArmor.Contains(armor))
            {
                OwnedArmor.Remove(armor);
                for (int i = 0; i < armors.Length; i++)
                    if (armors[i] == armor)
                        armors[i] = ArmorType.None;
            }
        }
        public int GetArmorResistance()
        {
            return ArmorResistance[(byte)armors[0]] + ArmorResistance[(byte)armors[1]] + ArmorResistance[(byte)armors[2]] + ArmorResistance[(byte)armors[3]];
        }
    }

    public class MetPeople
    {
        public bool sleepy = false;
        public bool bufff = false;
        public bool _float = false;
        public bool shoppohs = false;
    }

    public class PlayerStat
    {
        public int Health;
        public int MaxHealth;
        public long Money;
        public int Strength;
        public int Attack1;
        public int Attack2;
        public int Attack3;
        public PlayerStat()
        {
            Money = 0;
            Health = 100;
            MaxHealth = 100;
            Strength = 25;
            BuildAttackValues();
        }

        public void IncreaseHealth(int amount)
        {
            Health = Math.Clamp(Health + amount, 0, MaxHealth);
        }

        public void DecreaseHealth(int amount)
        {
            Health = Math.Clamp(Health - amount, 0, MaxHealth);
        }
        public void BuildAttackValues()
        {
            Attack1 = Strength / 3;
            Attack2 = (int)Math.Round(Strength * 0.8d);
            Attack3 = (int)Math.Round(Strength * 0.3d);
        }
    }

    public class MonsterStat // Based on ticks of 25ms
    {
        public int Health;
        int Speed;
        public int RewardStrength;
        public int RewardMoney;
        double MonsterStrength;
        public int Cooldown;
        public short DisplayRecentAttack;
        private Random random = new Random();
        public MonsterStat(int strengthinput)
        {
            // Split here, match them
            Health = strengthinput * random.Next(3, 7);
            Speed = 1;
            RewardStrength = (int)Math.Round(Health * 0.045m);
            RewardMoney = (int)Math.Round(Health * 0.16m);
            MonsterStrength = strengthinput * (random.Next(3, 7) / 9d);
            Cooldown = random.Next(15, 55) / Speed;
            DisplayRecentAttack = 0;
        }
        public int TryAttack()
        {

            DisplayRecentAttack = short.Max((short)(DisplayRecentAttack - 1), 0);
            if (Cooldown <= 0)
            {
                Cooldown = random.Next(15, 55) / Speed;
                DisplayRecentAttack = 20; // 15 * 25 ms of time for the displaying if attacked
                return (int)Math.Round(MonsterStrength * (random.Next(3, 6) / 10d)); //( 3 to 6 / 10) times monster stength, like if strength was 25 it is 7.5 dmg to 15 dmg
            }
            Cooldown--;
            return 0;
        }
    }
    enum MapItem : byte
    {
        Empty = 0,
        Player = 1,
        Wall = 2,
        AutoDoor = 3,
        Wardrobe = 4,
        Sleepy = 5,
        Bufff = 6,
        Float = 7,
        ShoppohS = 8,
        Bed = 9,
        Toilet = 10,
        Fighting = 11
    }

    static string QuarterPosition(decimal input, decimal goal)
    {
        if (goal <= 0) return "○"; // invalid goal safeguard
        if (input <= 0) return "○";

        decimal p = input / goal;
        if (p < 0.25m) return "◔";
        if (p < 0.50m) return "◑";
        if (p < 0.75m) return "◕";
        return "●";
    }

    static void ClearKeyBuf()
    {
        while (Console.KeyAvailable)
            Console.ReadKey(true);
    }
    static async Task Tell(string tosay, bool withClear, bool witheffect)
    {
        ClearKeyBuf();
        if (withClear)
            Console.Clear();
        if (witheffect)
        {
            int del = 700 / tosay.Length;
            foreach (char c in tosay)
            {
                Console.Write(c);
                await Task.Delay(del);
            }

        }
        else
            Console.Write(tosay);
        await Task.Delay(200);
        ClearKeyBuf();
        Console.ReadKey(true);
    }
    static async Task Say(string tosay, bool witheffect, int fulltime)
    {
        if (witheffect)
        {
            int del = fulltime / tosay.Length;
            foreach (char c in tosay)
            {
                Console.Write(c);
                await Task.Delay(del);
            }
        }
        else
            Console.Write(tosay);
        ClearKeyBuf();
    }

    static Func<short, short, short> IndexPos = (short x, short y) => (short)((y * width) + x);
    static string ConvertMapItem(MapItem mapitem) => mapitem switch
    {
        MapItem.Empty => " ",
        MapItem.Wall => "@",
        MapItem.AutoDoor => "=",
        MapItem.Player => "⚜️",
        MapItem.Sleepy => "🫩",
        MapItem.ShoppohS => "⌂",
        MapItem.Float => "🕴",
        MapItem.Bufff => "🏋",
        MapItem.Fighting => "🗡️",
        MapItem.Toilet => "𐃅",
        MapItem.Bed => "🛏️",
        MapItem.Wardrobe => "🀆",
        _ => "?"
    };
    class PlayerMoveCoolDown
    {
        public decimal Move1 = 0;
        public decimal Move2 = 0;
        public decimal Move3 = 5;
    }

    static async Task<(bool, int, int)> Fight()
    {
        playerstat.BuildAttackValues();
        Console.Clear();
        string arena = @"+====================================+
 ||/                              \||
 ||                                ||
//__________________________________\\
▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓▒▓";

        MonsterStat monster = new MonsterStat(playerstat.Strength);
        PlayerMoveCoolDown pcooldown = new PlayerMoveCoolDown();
        short PlayerDisplayRecentAttack = 0;
        bool playerWon = false;
        async Task Input()
        {
            char input;
            if (Console.KeyAvailable)
            {
                input = (await Task.Run(() => Console.ReadKey(true))).KeyChar;
                switch (input)
                {
                    case 'a':
                        if (pcooldown.Move1 <= 0)
                        {
                            PlayerDisplayRecentAttack = 20;
                            pcooldown.Move1 = 1;
                            monster.Health -= playerstat.Attack1;
                        }
                        break;
                    case 's':
                        if (pcooldown.Move2 <= 0)
                        {
                            PlayerDisplayRecentAttack = 20;
                            pcooldown.Move2 = 4;
                            monster.Health -= playerstat.Attack2;
                        }
                        break;
                    case 'd':
                        if (pcooldown.Move3 <= 0)
                        {
                            PlayerDisplayRecentAttack = 20;
                            pcooldown.Move1 = Math.Min(pcooldown.Move1 + 1, 2);
                            pcooldown.Move2 = Math.Min(pcooldown.Move2 + 1, 4);
                            pcooldown.Move3 = 0.6m;
                            monster.Health -= playerstat.Attack3;
                        }
                        break;

                }
            }

        }
        Func<short, string> ShowMoveStatus = (short time) => time > 0 ? "▦" : "□";
        CancellationTokenSource cts = new CancellationTokenSource();
        async Task Show()
        {
            Console.WriteLine($@"Hp {playerstat.Health}{ShowMoveStatus(PlayerDisplayRecentAttack)}                        Hp {monster.Health}{ShowMoveStatus(monster.DisplayRecentAttack)}
{arena}
[🁣] {QuarterPosition(pcooldown.Move1, 1)}A [🁢]{QuarterPosition(pcooldown.Move2, 4)}S [⏣]{QuarterPosition(pcooldown.Move3, 0.6m)}D");
            Console.SetCursorPosition(7, 4);
            Console.Write(new string(ConvertMapItem(MapItem.Player)));
            Console.SetCursorPosition(29, 4);
            Console.Write("🀫");
            Console.SetCursorPosition(38, 5);
            await Task.Delay(25);
        }
        int attack;
        async Task Ticking()
        {
            attack = monster.TryAttack();
            if (attack > 0)
            {
                playerstat.DecreaseHealth(Math.Max(attack - armor.GetArmorResistance(), 2));
            }
            pcooldown.Move1 = Math.Max(pcooldown.Move1 - 0.025m, 0);
            pcooldown.Move2 = Math.Max(pcooldown.Move2 - 0.025m, 0);
            pcooldown.Move3 = Math.Max(pcooldown.Move3 - 0.025m, 0);
            PlayerDisplayRecentAttack = short.Max((short)(PlayerDisplayRecentAttack - 1), 0);
            if (playerstat.Health <= 0)
            {
                playerWon = false;
                cts.Cancel();
            }
            else if (monster.Health <= 0)
            {
                playerWon = true;
                cts.Cancel();
            }
            await Task.Delay(25);
        }
        async Task Logic(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                await Input();
                await Ticking();
                await Task.Delay(25);
            }
        }
        async Task Display(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                await Show();
                await Task.Delay(10);
                Console.Clear();
            }

        }
        Task[] tasks = { Display(cts.Token), Logic(cts.Token) };
        await Task.WhenAll(tasks);
        Console.Clear();
        return (playerWon, monster.RewardMoney, monster.RewardStrength);
    }


    static void Display()
    {
        Console.WriteLine($"Hp {playerstat.Health,-5:G}|{playerstat.Money,7:C}");
        for (int i = 0; i < currentmap.Length; i++)
        {
            Console.Out.Write(ConvertMapItem(currentmap[i]));
            if ((i + 1) % width == 0)
                Console.WriteLine();
        }
        Console.Out.Flush();
    }

    static bool AskYN(string question)
    {
        Console.WriteLine(question);
        string read = "";
        bool askedMultiple = false;
        do
        {
            if (askedMultiple) Console.WriteLine(question);
            read = Console.ReadLine() ?? "".ToLower();
            askedMultiple = true;
        } while (read != "y" && read != "n");
        return read == "y" ? true : false;
    }
    static async Task DoAction(MapItem index)
    {
        switch (index)
        {
            case MapItem.Sleepy:
                if (!met.sleepy) // Sort of like ! (not) met . (met (thing)) sleepy
                {
                    await Say("Zuuuzzz-z-z-z-uh", true, 400);
                    await Task.Delay(300);
                    await Say(", WhaaaaaaAA?\n", true, 1000);
                    await Task.Delay(1300);
                    await Say("So your duh new SPRINKLE in this place.. ", true, 500);
                    await Task.Delay(2300);
                    await Say("Well im", true, 500);
                    await Say(" Sleepy (🫩 )\n", true, 300);
                    await Say("It good to meet you.. I can give you stuff i guezzzzz---.....(Ok/Lol) ", false, 600);
                    bool notAskedProper = false;
                    string sleepyInput;
                    do
                    {
                        if (notAskedProper == true) { await Say("Zzzzzzz-... Whutt??", true, 500); await Say(" Say it? ", true, 500); }
                        sleepyInput = Console.ReadLine() ?? "".ToLower();
                        sleepyInput = sleepyInput == null ? "" : sleepyInput;
                        if (sleepyInput == "ok" || sleepyInput == "lol")
                        {
                            Console.WriteLine(sleepyInput == "ok" ? "Yeaaaah" : "Beiiguh sleeeepppyyyy is NOOOT funny");
                            Console.ReadKey();
                            notAskedProper = false;
                        }
                        else
                            notAskedProper = true;

                    } while (notAskedProper == true);
                    met.sleepy = true;
                }
                else
                {
                    await Tell("Hullozzz--... its me Sleepy (🫩 )", false, true);
                }
                break;
            case MapItem.Toilet:
                await Tell("Uhhhhh, its broken. (𐃅 )", false, true);
                break;
            case MapItem.Bufff:
                if (!met.bufff)
                {
                    string bufffInput;
                    bool notAskedProper = false;
                    await Say("Hey you there!! You WEAKLING SPRINKLE! ", true, 600);
                    await Task.Delay(300);
                    await Say("I am the STRONGEST OF ALL IN THE JAIL... \n", true, 400);
                    await Task.Delay(1300);
                    await Say("IM", true, 200);
                    await Task.Delay(300);
                    await Tell(" BUFFF (🏋 )\n", false, true);
                    await Say("Get to the GYYYYYM AND GET RIPPED LIKE ME (Ok/Uh no) ", true, 600);
                    do
                    {
                        if (notAskedProper == true) Console.Write("Aye! WHAT DID YOU SAY TO MEE?");
                        bufffInput = Console.ReadLine() ?? "".ToLower();
                        if (bufffInput == "ok")
                        {
                            await Tell("NOW YOU PROMISE THAT, COME BACK AND EXERSIZE SOME TIME SOON!!", false, true);
                            notAskedProper = false;
                        }
                        else if (bufffInput == "uh no")
                        {
                            await Tell("WELL GET STRROOONG OR NEVER FIGHT, POTATO!", false, true);
                            notAskedProper = false;
                        }
                        else
                            notAskedProper = true;

                    } while (notAskedProper == true);
                    met.bufff = true;
                }

                else
                {
                    await Tell("SUP" + (playerstat.Strength >= 250 ? " BRO, GET BUFFIN WITH ME" : " POTATO, GET STRONNGGA"), false, true);
                    //EnterGym();
                }
                break;
            case MapItem.Bed:
                Console.Clear();
                currentmap[IndexPos(x, y)] = MapItem.Empty;
                Display();
                Task[] tasks = { Say("Sleeping...... . .\n", true, 3900), Task.Delay(4000) };
                await Task.WhenAll(tasks);
                Console.Clear();
                currentmap[IndexPos(x, y)] = MapItem.Player;
                Display();
                playerstat.IncreaseHealth(40);
                await Tell("You got 40 health!", false, true);
                break;
            case MapItem.ShoppohS:
                Console.Clear();
                Console.Write(@"                  /      \
                 | O    O |
                  \      /   Sorry, nothing in the shop
                  /      \
[     ]          /        \              <>
[][][][][][][][][[][][][][][][][][][][][][][][][][][]");
                Console.ReadKey();
                Console.Clear();
                Display();
                break;
            case MapItem.Fighting:
                (bool, int, int) res = await Fight();
                await Tell(@$"Strength {playerstat.Strength} --
Money {playerstat.Money} >>", true, true);
                if (res.Item1)
                {
                    Console.Clear();
                    Console.Write($@"Strength {playerstat.Strength} >> {playerstat.Strength + res.Item3}
Money {playerstat.Money} >> {playerstat.Money + res.Item2}");
                    playerstat.Money += res.Item2;
                    playerstat.Strength += res.Item3;
                }
                else
                {
                    Console.Clear();
                    Console.Write($@"Strength {playerstat.Strength} -- {playerstat.Strength}
Money {playerstat.Money} >> {playerstat.Money}");
                }
                Console.ReadKey();
                break;
            case MapItem.Float:
                await Tell("Hey, its me float... I cant save your progress, I got caught", false, true);
                break;
            case MapItem.Wardrobe:
                bool exit = false;
                int selection = 0;
                ArmorType[] OwnedArmorArr = OwnedArmor.OrderBy(v => v).ToArray();
                ConsoleKey key = ConsoleKey.None;
                do
                {
                    if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow)
                        selection = Math.Max(selection - 1, 0);
                    else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow)
                        selection = Math.Min(selection + 1, 3);
                    else if (key == ConsoleKey.W || key == ConsoleKey.UpArrow)
                    {
                        int idx = Array.IndexOf(OwnedArmorArr, armor.armors[selection]);
                        armor.WearArmor(selection, OwnedArmorArr[Math.Clamp(idx + 1, 0, OwnedArmorArr.Length - 1)]);
                    }
                    else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow)
                    {
                        int idx = Array.IndexOf(OwnedArmorArr, armor.armors[selection]);
                        armor.WearArmor(selection, OwnedArmorArr[Math.Clamp(idx - 1, 0, OwnedArmorArr.Length - 1)]);
                    }
                    else if (key == ConsoleKey.E)
                    {
                        exit = true;
                        continue;
                    }
                    Console.Clear();
                    Console.WriteLine("Press E to exit");
                    for (int i = 0; i < 4; i++)
                    {
                        Console.Write($"{armor.ArmorTypeAsString(armor.armors[i])}{(i == selection ? " <--" : "")}");
                        Console.WriteLine();
                    }
                    Console.WriteLine($"\nThis piece gives {(armor.armors[selection] == ArmorType.None ? "no" : ArmorResistance[(byte)armor.armors[selection]])} resistance");
                    key = Console.ReadKey().Key;
                } while (!exit);
                armor.AddArmor(ArmorType.Cloth);
                armor.AddArmor(ArmorType.ScrapCloth);
                armor.WearArmor(0, ArmorType.ScrapCloth);
                break;
        }
    }

    static async Task MovePlayer(char input)
    {
        switch (input) // Move the player
        {
            case 'w':
                if (currentmap[IndexPos(x, (short)(y - 1))] == MapItem.Empty)
                {
                    currentmap[IndexPos(x, y)] = MapItem.Empty;
                    currentmap[IndexPos(x, (short)(y - 1))] = MapItem.Player;
                    y--;
                }
                else
                    await DoAction(currentmap[IndexPos(x, (short)(y - 1))]);
                break;
            case 's':
                if (currentmap[IndexPos(x, (short)(y + 1))] == MapItem.Empty)
                {
                    currentmap[IndexPos(x, y)] = MapItem.Empty;
                    currentmap[IndexPos(x, (short)(y + 1))] = MapItem.Player;
                    y++;
                }
                else
                    await DoAction(currentmap[IndexPos(x, (short)(y + 1))]);
                break;
            case 'a':
                if (currentmap[IndexPos((short)(x - 1), y)] == MapItem.Empty)
                {
                    currentmap[IndexPos(x, y)] = MapItem.Empty;
                    currentmap[IndexPos((short)(x - 1), y)] = MapItem.Player;
                    x--;
                }
                else
                    await DoAction(currentmap[IndexPos((short)(x - 1), y)]);
                break;
            case 'd':
                if (currentmap[IndexPos((short)(x + 1), y)] == MapItem.Empty)
                {
                    currentmap[IndexPos(x, y)] = MapItem.Empty;
                    currentmap[IndexPos((short)(x + 1), y)] = MapItem.Player;
                    x++;
                }
                else
                    await DoAction(currentmap[IndexPos((short)(x + 1), y)]);
                break;
        }

    }


    static short x = 24;
    static short y = 1;

    const short height = 15;
    const short width = 50;

    static MapItem[] currentmap = new MapItem[750];
    static MetPeople met = new MetPeople();
    static PlayerStat playerstat = new PlayerStat();
    static HashSet<ArmorType> OwnedArmor = new HashSet<ArmorType> { ArmorType.None };
    static Armor armor = new Armor();
    static async Task Main()
    {
        await Tell("This is in testing so armor is applied after you exit th closet for now. (Press any key)", true, true);
        ClearKeyBuf();
        await Tell("\nSprinkletraz is a game I made up where its some prisoners trying to escape a island as a game (Very huge goals for a terminal to handle :|)", false, true);
        ClearKeyBuf();
        await Tell("\nAlso its WASD to move and ASD in the fighting arena", false, true);
        Console.Clear();
        for (short i = 0; i < width; i++)
            currentmap[IndexPos(i, 0)] = MapItem.Wall;
        for (short i = 0; i < width; i++)
            currentmap[IndexPos(i, height - 1)] = MapItem.Wall;
        for (short i = 0; i < height; i++)
            currentmap[IndexPos(0, i)] = MapItem.Wall;
        for (short i = 0; i < height; i++)
            currentmap[IndexPos(width - 1, i)] = MapItem.Wall;
        currentmap[IndexPos(x, y)] = MapItem.Player;
        currentmap[IndexPos(7, 2)] = MapItem.Sleepy;
        currentmap[IndexPos(1, height - 2)] = MapItem.Bed;
        currentmap[IndexPos(17, height - 9)] = MapItem.ShoppohS;
        currentmap[IndexPos(27, height - 2)] = MapItem.Float;
        currentmap[IndexPos(3, height - 2)] = MapItem.Toilet;
        currentmap[IndexPos(15, height - 2)] = MapItem.Bufff;
        currentmap[IndexPos(38, height - 2)] = MapItem.Fighting;
        currentmap[IndexPos(6, height - 2)] = MapItem.Wardrobe;
        while (true)
        {
            Display();
            await MovePlayer(Console.ReadKey(true).KeyChar);
            Console.Clear();
        }


    }
}