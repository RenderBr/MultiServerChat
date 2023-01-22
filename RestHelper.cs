using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Rests;
using TShockAPI;

namespace MultiServerChat
{
    public static class RestHelper
    {
        public static async void SendChatMessage(TSPlayer ply, string formatted_text)
        {
            
                bool failure = false;
                var message = new Message()
                {
                    Text = String.Format(MultiServerChat.Config.ChatFormat,
                                            TShock.Config.Settings.ServerName,
                                            formatted_text),
                    Red = ply.Group.R,
                    Green = ply.Group.G,
                    Blue = ply.Group.B
                };

                var bytes = Encoding.UTF8.GetBytes(message.ToString());
                var base64 = Convert.ToBase64String(bytes); 
            
                foreach(string newUrl in MultiServerChat.Config.RestURLs)
                {
                    string voteUrl = newUrl + "/jl?token=" + MultiServerChat.Config.Token + "&message=" + message;


                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            using (HttpResponseMessage res = await client.GetAsync(voteUrl))
                            {
                                using (HttpContent content = res.Content)
                                {
                                    var data = await content.ReadAsStringAsync();

                                    if (data != null)
                                    {
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);


                    }

            }



        }

        

        public async static void SendJoinMessage(TSPlayer ply)
        {

            bool failure = false;

            string text;

            if (TShock.Config.Settings.EnableGeoIP && TShock.Geo != null)
                text = String.Format(MultiServerChat.Config.GeoJoinFormat, TShock.Config.Settings.ServerName, ply.Name, ply.Country);
            else
                text = string.Format(MultiServerChat.Config.JoinFormat, TShock.Config.Settings.ServerName, ply.Name);

            var message = new Message()
            {
                Text = text,
                Red = Color.Yellow.R,
                Green = Color.Yellow.G,
                Blue = Color.Yellow.B
            };

            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            var base64 = Convert.ToBase64String(bytes);
            foreach (string newUrl in MultiServerChat.Config.RestURLs)
            {
                string voteUrl = newUrl + "/jl?token=" + MultiServerChat.Config.Token + "&message=" + message;


                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage res = await client.GetAsync(voteUrl))
                        {
                            using (HttpContent content = res.Content)
                            {
                                var data = await content.ReadAsStringAsync();

                                if (data != null)
                                {
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);


                }

            }

        }

        public async static void SendLeaveMessage(TSPlayer ply)
        {
            bool failure = false;

            string text;

            var message = new Message()
            {
                Text =
            String.Format(MultiServerChat.Config.LeaveFormat, TShock.Config.Settings.ServerName, ply.Name),
                Red = Color.Yellow.R,
                Green = Color.Yellow.G,
                Blue = Color.Yellow.B
            };

            var bytes = Encoding.UTF8.GetBytes(message.ToString());
            var base64 = Convert.ToBase64String(bytes);
            foreach (string newUrl in MultiServerChat.Config.RestURLs)
            {
                string voteUrl = newUrl + "/jl?token=" + MultiServerChat.Config.Token + "&message=" + message;


                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage res = await client.GetAsync(voteUrl))
                        {
                            using (HttpContent content = res.Content)
                            {
                                var data = await content.ReadAsStringAsync();

                                if (data != null)
                                {
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);


                }

            }

        }

        public static dynamic RecievedMessage(RestRequestArgs args)
        {

            var message = JsonConvert.DeserializeObject<dynamic>(args.Parameters["message"]);
           
            Console.WriteLine(message);

            return message;
        }
    }
}
