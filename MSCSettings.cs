using Auxiliary.Configuration;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Connector
{
	public class MSCSettings : ISettings
	{
		[JsonPropertyName("RestURLS")]
		public List<string> RestURLs = new List<string>();

		[JsonPropertyName("Token")]
		public string Token = "abcdef";

		[JsonPropertyName("ChatFormat")]
		public string ChatFormat = "[{0}] {1}";

		[JsonPropertyName("JoinFormat")]
		public string JoinFormat = "[{0}] {1} has joined.";

		[JsonPropertyName("GeoJoinFormat")]
		public string GeoJoinFormat = "[{0}] {1} ({2}) has joined.";

		[JsonPropertyName("LeaveFormat")]
		public string LeaveFormat = "[{0}] {1} has left.";

		[JsonPropertyName("SendChat")]
		public bool SendChat = true;

		[JsonPropertyName("SendJoinLeave")]
		public bool SendJoinLeave = true;

		[JsonPropertyName("DisplayChat")]
		public bool DisplayChat = true;

		[JsonPropertyName("DisplayJoinLeave")]
		public bool DisplayJoinLeave = true;

		[JsonPropertyName("ServerName")]
		public string ServerName = "MSC Message";

	}
}
