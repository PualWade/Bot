using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace Bot
{
    class Program
    {
        static IrcClient Irc = new IrcClient();
        const int port = 6667;
        static string ip = "irc.root-me.org";
        static string nick = "botTest";
        static string realname = "real " + nick;
        static string channel = "#root-me_challenge";

        static string serverName = "Paul_Wade";

    
        static void Main(string[] args)
        {
            Irc.Encoding = Encoding.UTF8;
            Irc.SendDelay = 200;
            Irc.ActiveChannelSyncing = true;
            Irc.OnRawMessage += Irc_OnRawMessage;
            Irc.OnErrorMessage += Irc_OnErrorMessage;
            Connect();
            while (true)
            {
                string msg = Console.ReadLine();
                Irc.SendMessage(SendType.Message, channel, msg);
                Console.WriteLine(msg);
            }
            
        }

        private static void Irc_OnErrorMessage(object sender, IrcEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.Data.Message);
        }

        private static void Irc_OnRawMessage(object sender, IrcEventArgs e)
        {
            try
            {
                if (serverName == e.Data.Nick && e.Data.MessageArray[0] == "myPassword" && e.Data.Nick != null && e.Data.MessageArray != null)
                {
                    switch (e.Data.MessageArray[1])
                    {
                        case "ping":
                            Console.WriteLine("{0}   {1}: {2}", DateTime.Now.TimeOfDay, e.Data.Nick, e.Data.MessageArray[1]);
                            break;
                        case "command1":
                            Process.Start(@"C:\Windows\system32\cmd.exe");
                            break;
                    }
                }
                else Console.WriteLine(e.Data.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Connect()
        {
            Irc.Connect(ip, port);
            Irc.Login(nick, realname);
            Irc.RfcJoin(channel);
            new Thread(new ThreadStart(Irc.Listen)).Start();
        }
    }
}
