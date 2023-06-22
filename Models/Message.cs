using Newtonsoft.Json;

namespace Connector
{
	public class Message
	{
		public string Text { get; set; }
		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }

		public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

		public static Message FromJson(string js) => JsonConvert.DeserializeObject<Message>(js);
	}
}
