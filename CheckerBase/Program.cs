using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using Leaf.xNet;

namespace BoltPanel
{
    internal class Program
    {

        public static int totalChecked, cpm, hits, fails, errors, threads, comboIndex;
        public static string proxyType, webhooklink;
        public static List<string> proxies,combos = new List<string>();
        public static int Proxytotal, Combototal;
        public static bool webhook;


        static void Main(string[] args)
        {
            #region startup
            ASCII();

            Utils.print("Select Combo", "");
            ComboLoad();


            Utils.print("Select Proxylist", "");
            ProxyLoad();

            for (; ; )
            {
                Utils.print("Select Proxy Type", "\n");
                Utils.print("1", "HTTP\n");
                Utils.print("2", "SOCKS4\n");
                Utils.print("3", "SOCKS5\n");
                Utils.print(">", "");
                var readProxy = System.Console.ReadLine();
                switch (readProxy)
                {
                    case "1":
                        proxyType = "HTTP";
                        break;

                    case "2":
                        proxyType = "SOCKS4";
                        break;

                    case "3":
                        proxyType = "SOCKS5";
                        break;
                    default:
                        continue;
                }
                break;
            }

            Console.Clear();
            ASCII();

            Utils.print("How many threads do you want to use", "\n");
            Utils.print(">", "");
            bool validInput = false;
            while (!validInput)
            {
                try
                {
                    threads = Convert.ToInt32(System.Console.ReadLine());
                    validInput = true;
                }
                catch
                {
                    Utils.print("Error! Input a number", "");
                    Console.Write("    [", Color.White);
                    Console.Write("Error! Input a number", Color.Red);
                    Console.Write("]\n", Color.White);
                }
            }

            Console.Clear();
            ASCII();

            Utils.print("Do you want to use webhook? (Y/N | default: No)", "\n");
            Utils.print(">", "");
            var webhookquestion = System.Console.ReadLine();
            switch (webhookquestion)
            {
                case "Y":
                    Utils.print("Webhook link:", "\n");
                    Utils.print(">", "");
                    webhook = true;
                    webhooklink = System.Console.ReadLine();
                    break;
                case "N":
                    webhook = false;
                    break;
                default:
                    webhook = false;
                    break;
            }

            #endregion
            #region start

            Utils.Initialize();
            var num = 0;
            while (num <= threads)
            {
                new Thread(new ThreadStart(check)).Start();
                num = num + 1;
            }

            Task.Factory.StartNew(delegate { UpdateConsole(); });
            Console.ReadLine();
            #endregion
        }

        public static void UpdateConsole()
        {
            var lastChecks = totalChecked;
             while(Thread.CurrentThread.IsAlive)
            {
                cpm = totalChecked - lastChecks;
                lastChecks = totalChecked;
                Console.Clear();
                ASCII();
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.Write("    [", Color.White);
                Console.Write("HITS", Color.LimeGreen);
                Console.Write($"] {hits}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("FAILS", Color.Red);
                Console.Write($"] {fails}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("ERRORS", Color.DarkOrange);
                Console.Write($"] {Program.errors}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("TOTAL", Color.Aquamarine);
                Console.Write($"] {totalChecked}\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("CPM", Color.RoyalBlue);
                Console.Write($"] {Program.cpm * 60}\n\n", Color.White);
                Console.Write("    [", Color.White);
                Console.Write("INFO", Color.Green);
                Console.Write("] discord.gg/DiogoAlts \n",Color.White);

                Console.Title = "DiogoBase - Hits: " + hits + " | Fails: " + fails + " | Errors: " + errors + " CPM: " + cpm;

                Thread.Sleep(1000);
            }
        }

        public static void check()
        {
            while (Thread.CurrentThread.IsAlive)
            { 
                using (var req = new HttpRequest())
                {
                    try
                    {
                        string proxy = proxies.ElementAt<string>(new Random().Next(proxies.Count));
                        var array = combos[comboIndex].Split(':', ';', '|');
                        Interlocked.Increment(ref comboIndex);

                        totalChecked++;

                        switch (proxyType)
                        {
                            case "HTTP":
                                req.Proxy = HttpProxyClient.Parse(proxy);
                                req.Proxy.ConnectTimeout = 5000;
                                break;
                            case "SOCKS4":
                                req.Proxy = Socks4ProxyClient.Parse(proxy);
                                req.Proxy.ConnectTimeout = 5000;
                                break;
                            case "SOCKS5":
                                req.Proxy = Socks5ProxyClient.Parse(proxy);
                                req.Proxy.ConnectTimeout = 5000;
                                break;
                        }

                        req.UserAgent = Http.RandomUserAgent();

                        // Hit
                        hits++;
                        Utils.AsResult("/Module_Name", array[0] + ":" + array[1]);
                        if (webhook)
                            Utils.sendTowebhook(array[0] + ":" + array[1], "Module name");

                    }
                    catch (Exception ex)
                    {
                        errors++;
                    }
                }
            }
        }

        public static void ASCII()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("              ██████╗ ██╗ █████╗  ██████╗  █████╗ ██████╗  █████╗  ██████╗███████╗", Color.Purple);
            Console.WriteLine("              ██╔══██╗██║██╔══██╗██╔════╝ ██╔══██╗██╔══██╗██╔══██╗██╔════╝██╔════╝", Color.Purple);
            Console.WriteLine("              ██║  ██║██║██║  ██║██║  ██╗ ██║  ██║██████╦╝███████║╚█████╗ █████╗", Color.Purple);
            Console.WriteLine("              ██║  ██║██║██║  ██║██║  ╚██╗██║  ██║██╔══██╗██╔══██║ ╚═══██╗██╔══╝", Color.Purple);
            Console.WriteLine("              ██████╔╝██║╚█████╔╝╚██████╔╝╚█████╔╝██████╦╝██║  ██║██████╔╝███████╗", Color.Purple);
            Console.WriteLine("              ╚═════╝ ╚═╝ ╚════╝  ╚═════╝  ╚════╝ ╚═════╝ ╚═╝  ╚═╝╚═════╝ ╚══════╝", Color.Purple);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void ComboLoad()
        {
            string fileName;
            var x = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Combo List";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                combos = new List<string>(File.ReadAllLines(fileName));
                Combototal = combos.Count();
                Console.Write("Selected ", Color.White);
                Console.Write(Combototal, Color.Purple);
                Console.Write(" Combos\n\n", Color.White);
            });
            x.SetApartmentState(ApartmentState.STA);
            x.Start();
            x.Join();
        }
           

        public static void ProxyLoad()
        {
            string fileName;
            var x = new Thread(() =>
            {
                var openFileDialog = new OpenFileDialog();
                do
                {
                    openFileDialog.Title = "Select Proxy List";
                    openFileDialog.DefaultExt = "txt";
                    openFileDialog.Filter = "Text files|*.txt";
                    openFileDialog.RestoreDirectory = true;
                    openFileDialog.ShowDialog();
                    fileName = openFileDialog.FileName;
                } while (!File.Exists(fileName));

                proxies = new List<string>(File.ReadAllLines(fileName));
                Proxytotal = proxies.Count();
                Console.Write("Selected ", Color.White);
                Console.Write(Proxytotal, Color.Purple);
                Console.Write(" Proxies\n\n", Color.White);
            });
            x.SetApartmentState(ApartmentState.STA);
            x.Start();
            x.Join();
        }
    }
}
