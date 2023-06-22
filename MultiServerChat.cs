using Auxiliary.Configuration;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Rests;
using System;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Connector
{
	[ApiVersion(2, 1)]
	public class MultiServerChat : TerrariaPlugin
	{
		public static MSCSettings Config;
		public override string Author => "Average";
		public override string Description => "Link chat messages across multiple servers";
		public override string Name => "Connector";
		public override Version Version => new Version(1, 0);

		public MultiServerChat(Main game) : base(game) { }

		public override void Initialize()
		{
			GeneralHooks.ReloadEvent += OnReload;
			PlayerHooks.PlayerChat += OnChat;
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			TShock.RestApi.Register(new SecureRestCommand("/msc", RestChat, "msc.canchat"));
			TShock.RestApi.Register(new SecureRestCommand("/jl", RestChat, "msc.canchat"));

			Config = Configuration<MSCSettings>.Load(nameof(MultiServerChat));
		}

		private void OnReload(ReloadEventArgs args) => Config = Configuration<MSCSettings>.Load(nameof(MultiServerChat));

		private object RestChat(RestRequestArgs args)
		{
			if (!Config.DisplayChat)
				return new RestObject();

			RestHelper.ReceivedMessage(args);

			Message message = JsonConvert.DeserializeObject<Message>(args.Parameters["message"]);
			TSPlayer.All.SendMessage(message.Text, message.Red, message.Green, message.Blue);

			return new RestObject();
		}

		private void OnChat(PlayerChatEventArgs args)
		{
			if (!Config.SendChat || args.Handled)
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

			if (ply == null || ply.SilentJoinInProgress)
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

		public void SendMessage(dynamic args)
		{
			TSPlayer.All.SendInfoMessage((string)args.Text);
			TSPlayer.All.SendMessage((string)args.Text, Color.Red);
		}

		private void OnLeave(LeaveEventArgs args)
		{
			if (!Config.DisplayJoinLeave)
				return;

			TSPlayer ply = TShock.Players[args.Who];
			if (ply == null || ply.SilentKickInProgress || ply.State < 3)
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
