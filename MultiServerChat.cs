using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Auxiliary.Configuration;
using HttpServer;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using Rests;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace MultiServerChat
{
    [ApiVersion(2, 1)]
    public class MultiServerChat : TerrariaPlugin
    {
        internal static MSCSettings Config;
        private string savePath;

        public override string Author => "Average";
        public override string Description => "Link chat messages across multiple servers";
        public override string Name => "Connector";
        public override Version Version => new Version(1, 0);

        public MultiServerChat(Main game) : base(game) { }


        public override void Initialize()
        {
            savePath = Path.Combine(TShock.SavePath, "Connector.json");
            GeneralHooks.ReloadEvent += OnReload;
            PlayerHooks.PlayerChat += OnChat;
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            TShock.RestApi.Register(new SecureRestCommand("/msc", RestChat, "msc.canchat"));
            TShock.RestApi.Register(new SecureRestCommand("/jl", RestChat, "msc.canchat"));
        }
        
        private void OnReload(ReloadEventArgs args)
        {
            if (args.Player.Group.HasPermission("msc.reload"))
            {
                Config = Configuration<MSCSettings>.Load(nameof(MultiServerChat));
            }
        }

        private object RestChat(RestRequestArgs args)
        {
            if (!Config.DisplayChat)
                return new RestObject();

            RestHelper.RecievedMessage(args);

            Message message = JsonConvert.DeserializeObject<Message>(args.Parameters["message"]);
            TSPlayer.All.SendMessage(message.Text, message.Red, message.Green, message.Blue);

            return new RestObject();
        }

        private void OnChat(PlayerChatEventArgs args)
        {
            if (!Config.SendChat)
                return;
            if (args.Handled)
                return;

            try
            {
                RestHelper.SendChatMessage(args.Player, args.TShockFormattedText);
            }
            catch
            {
                Console.WriteLine("One of the rest urls could not be interfaced with.");
            }
        }

        private void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            if (!Config.DisplayJoinLeave)
                return;

            TSPlayer ply = TShock.Players[args.Who];

            if (ply == null)
                return;

            if (ply.SilentJoinInProgress)
                return;

            try
            {
                RestHelper.SendJoinMessage(ply);
            }
            catch
            {
                Console.WriteLine("One of the rest urls could not be interfaced with.");
            }
        }

        public void sendMessage(dynamic args)
        {
            TSPlayer.All.SendInfoMessage((string)args.Text);
            TSPlayer.All.SendMessage((string)args.Text, Color.Red);
        }

        private void OnLeave(LeaveEventArgs args)
        {
            if (!Config.DisplayJoinLeave)
                return;

            TSPlayer ply = TShock.Players[args.Who];
            if (ply == null)
                return;

            if (ply.SilentKickInProgress || ply.State < 3)
                return;

            try
            {
                RestHelper.SendLeaveMessage(ply);
            }
            catch
            {
                Console.WriteLine("One of the rest urls could not be interfaced with.");
            }
        }
    }
}
