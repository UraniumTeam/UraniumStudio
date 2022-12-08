using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace UraniumStudio.Data;

public class JsonParser
{
	public static List<string> ParseUpsJson(string path)
	{
		List<string?> resultStrings = new();
		using (var reader = new StreamReader(path))
		{
			string json = reader.ReadToEnd();
			var threads = JObject.Parse(json);
			var threadsValues = threads.Properties().Values().Values<string>();
			resultStrings.AddRange(threadsValues.Select(value => value.ToString()));
		}

		return resultStrings!;
	}
}
