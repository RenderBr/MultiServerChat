using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Rests;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace Connector
{
	public static class RestHelper
	{
		public static async Task SendChatMessage(TSPlayer ply, string formattedText)
		{
			var message = new Message()
			{
				Text = $"{MultiServerChat.Config.ChatFormat} {MultiServerChat.Config.ServerName} {formattedText}",
				Red = ply.Group.R,
				Green = ply.Group.G,
				Blue = ply.Group.B
			};

			var bytes = Encoding.UTF8.GetBytes(message.ToString());
			var base64 = Convert.ToBase64String(bytes);

			var tasks = new List<Task>();
			using (var client = new HttpClient())
			{
				foreach (string url in MultiServerChat.Config.RestURLs)
				{
					string requestUrl = $"{url}/jl?token={MultiServerChat.Config.Token}&message={base64}";
					tasks.Add(Task.Run(() => SendRequestAsync(client, requestUrl)));
				}

				await Task.WhenAll(tasks);
			}
		}

		public static async Task SendJoinMessage(TSPlayer ply)
		{
			string text;

			if (TShock.Config.Settings.EnableGeoIP && TShock.Geo != null)
				text = $"{MultiServerChat.Config.GeoJoinFormat} {MultiServerChat.Config.ServerName} {ply.Name} {ply.Country}";
			else
				text = $"{MultiServerChat.Config.JoinFormat} {MultiServerChat.Config.ServerName} {ply.Name}";

			var message = new Message()
			{
				Text = text,
				Red = Color.Yellow.R,
				Green = Color.Yellow.G,
				Blue = Color.Yellow.B
			};

			var bytes = Encoding.UTF8.GetBytes(message.ToString());
			var base64 = Convert.ToBase64String(bytes);

			var tasks = new List<Task>();
			using (var client = new HttpClient())
			{
				foreach (string url in MultiServerChat.Config.RestURLs)
				{
					string requestUrl = $"{url}/jl?token={MultiServerChat.Config.Token}&message={base64}";
					tasks.Add(Task.Run(() => SendRequestAsync(client, requestUrl)));
				}

				await Task.WhenAll(tasks);
			}
		}

		public static async Task SendLeaveMessage(TSPlayer ply)
		{
			var message = new Message()
			{
				Text = $"{MultiServerChat.Config.LeaveFormat} {MultiServerChat.Config.ServerName} {ply.Name}",
				Red = Color.Yellow.R,
				Green = Color.Yellow.G,
				Blue = Color.Yellow.B
			};

			var bytes = Encoding.UTF8.GetBytes(message.ToString());
			var base64 = Convert.ToBase64String(bytes);

			var tasks = new List<Task>();
			using (var client = new HttpClient())
			{
				foreach (string url in MultiServerChat.Config.RestURLs)
				{
					string requestUrl = $"{url}/jl?token={MultiServerChat.Config.Token}&message={base64}";
					tasks.Add(Task.Run(() => SendRequestAsync(client, requestUrl)));
				}

				await Task.WhenAll(tasks);
			}
		}

		private static async Task SendRequestAsync(HttpClient client, string url)
		{
			try
			{
				using (HttpResponseMessage response = await client.GetAsync(url))
				{
					using (HttpContent content = response.Content)
					{
						var data = await content.ReadAsStringAsync();
						if (data != null)
						{
							// Handle the response data if needed
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public static dynamic ReceivedMessage(RestRequestArgs args) => JsonConvert.DeserializeObject<dynamic>(args.Parameters["message"]);
	}
}
