using System.Net;
using System.Text;
using System.IO;
using System.Threading;


class WebsocketsServer
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
        await Say("This program is a websocket server that serves files inside \"public\" and runs on localhost 8000 but ONLY for html", true, 1000);
        if (!HttpListener.IsSupported)
        {
            Console.WriteLine("Windows XP SP2 or Server 2003 or newer/better is required to use the HttpListener class.");
            return;
        }
        HttpListener host = new HttpListener();
        Console.WriteLine();
        host.Prefixes.Add("http://*:8000/");
        host.Start();
        Console.WriteLine(@"Server is running on port 8000.
It can be accessed on this machine via:
- http://localhost:8000/");
        while (true)
        {
            var client = await host.GetContextAsync();
            _ = Task.Run(() => FindClient(client));
        }

        Task FindClient(HttpListenerContext client)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            CancellationToken cancel = cts.Token;
            _ = Task.Run(() => HandleClient(client.Response, client.Request, cancel));
            return Task.CompletedTask;
        }
        async Task HandleClient(HttpListenerResponse client, HttpListenerRequest request, CancellationToken cancel)
        {
            string servingFile = Path.GetFullPath($"./public/{request.Url?.LocalPath[1..]}");
            byte[] buffer;
            var stream = client.OutputStream;
            if (servingFile.EndsWith('/'))
            {
                string[] commonEnds = new string[] {"index.html", "index.htm", "index.php", "index.cgi", "index.jsp",
                "default.html", "default.htm", "default.php", "default.cgi", "default.jsp",
                "home.html", "home.htm","home.php","home.cgi","home.jsp"};
                foreach (string end in commonEnds)
                {
                    if (Path.Exists(servingFile + end))
                    {
                        servingFile += end;
                        break;
                    }
                }//If theres no common end file, it will end
            }
            if (servingFile.Contains(".."))
            {
                if (Path.Exists(Path.GetFullPath($"./public/400.html")))
                    servingFile = Path.GetFullPath($"./public/400.html");
                else
                    await ServeEarly("Cannot go beyond website");
                client.StatusCode = 400;
            }
            if (!Path.Exists(servingFile))
            {
                if (Path.Exists(Path.GetFullPath($"./public/404.html")))
                    servingFile = Path.GetFullPath($"./public/404.html");
                else
                    await ServeEarly("Cannot find web page");
                client.StatusCode = 404;
            }
            client.StatusCode = 200;
            buffer = Encoding.UTF8.GetBytes((VerifyDoctypeHtml() == true) ? File.ReadAllText(servingFile) : "<!DOCTYPE html>\n" + File.ReadAllText(servingFile));
            await stream.WriteAsync(buffer, 0, buffer.Length);
            //End
            stream.Close();
            client.Close();
            async Task ServeEarly(string text)
            {
                buffer = Encoding.UTF8.GetBytes(text);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                //End early
                stream.Close();
                client.Close();
                return;
            }
            bool VerifyDoctypeHtml()
            {
                string? line;
                using (
                FileStream s = new FileStream(servingFile, FileMode.Open, FileAccess.Read))
                using (
                StreamReader verify = new StreamReader(s))
                    while ((line = verify.ReadLine()) != null)
                    {
                        if (line.ToLower().IndexOf("<!DOCTYPE html>") != -1)
                            return true;
                    }
                return false;
            }
        }
    }
}