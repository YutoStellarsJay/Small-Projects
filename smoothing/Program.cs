using System;
using System.Threading.Tasks;
class Program
{
    static void ClearKeyBuf()
    {
        while (Console.KeyAvailable)
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
    static async Task Main()
    {
        await Say("This program shows error/difference based smoothing, use keys 1-0 to control a little character, you can also edit main variables easily for it to adapt (Press any key)", true, 1000);
        Console.ReadKey(true);
        char[] show = new char[8];
        double position = 0;
        double origin = 0;
        double goal = 10;
        double easing = 2;
        Task[] tasks = new Task[] { Listen(), Output() };
        await Task.WhenAll(tasks);
        async Task Output()
        {
            Console.WriteLine(position);
            Array.Fill(show, ':');
            show[(int)Math.Round(position)] = 'O';
            Console.WriteLine(String.Join(' ', show));
            while (true)
            {
                position += Math.Round((goal - (position - origin)) / easing, 2);
                Console.WriteLine(Math.Round(position, 2));
                Array.Fill(show, ':');
                show[Math.Clamp((int)Math.Round(position) - 1, 0, show.Length - 1)] = 'O';
                Console.WriteLine(String.Join(' ', show));
                await Task.Delay(100);
            }
        }
        async Task Listen()
        {
            while (true)
            {
                char key = await Task.Run(() => Console.ReadKey(true).KeyChar);
                if (key > '0' && key <= '9')
                    goal = key - '0';
                else if (key == '0')
                    goal = 10;
            }
        }
    }
}